using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ExcelEnumAttribute : Attribute
{
    public string Value { get; }
    public ExcelEnumAttribute(string value)
    {
        Value = value;
    }
}

internal sealed class ContainerFieldInfo
{
    public FieldInfo Field;
    public SheetBindingAttribute Binding;
}

internal sealed class DataFieldInfo
{
    public FieldInfo Field;
    public ExcelParerAttribute Parser;
    public MultiColumnParserAttribute MultiParser;
    public string ColumnName;
}

internal sealed class SheetSectionInfo
{
    public string Name;
    public int Row;
    public int Column;
    public bool IsColumnBased;
}

public static class ExcelLoader
{
    private static readonly Regex SheetSectionRegex = new(
        @"^\[(?<name>[^\]]+)\](?<mode>\*?)$",
        RegexOptions.Compiled);

    private static readonly HashSet<string> ExcelExtensions = new(
        StringComparer.OrdinalIgnoreCase)
    {
        ".xls",
        ".xlsx",
        ".xlsm",
        ".xlsb",
        ".csv",
    };

    public static T LoadExcelFile<T>(T container, string path) where T : class
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"[ExcelLoader] File not found: {path}",
                path);
        }

        container ??= (T)Activator.CreateInstance(typeof(T));
        var containerFields = GetContainerFields(container);

        LoadExcel(container, path, containerFields);

        ValidateContainerFields(container, containerFields);

        return container;
    }

    public static T LoadAllExcelFiles<T>(T container, string folderPath, bool recursive = true) where T : class
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException(
                $"[ExcelLoader] Folder not found: {folderPath}");
        }

        container ??= (T)Activator.CreateInstance(typeof(T));
        var containerFields = GetContainerFields(container);

        var excelFiles = CollectExcelFiles(folderPath, recursive);

        for (int i = 0; i < excelFiles.Count; i++)
        {
            LoadExcel(container, excelFiles[i], containerFields);
        }

        ValidateContainerFields(container, containerFields);

        return container;
    }

    private static List<string> CollectExcelFiles(string folderPath, bool recursive)
    {
        var result = new List<string>(32);
        CollectExcelFilesInternal(folderPath, recursive, result);
        result.Sort(StringComparer.OrdinalIgnoreCase);
        return result;
    }

    private static void CollectExcelFilesInternal(string folderPath, bool recursive, List<string> result)
    {
        string[] files = Directory.GetFiles(
            folderPath,
            "*",
            SearchOption.TopDirectoryOnly);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);
            if (fileName.Length == 0 || fileName[0] == '~')
                continue;
            if (ExcelExtensions.Contains(Path.GetExtension(files[i])))
                result.Add(files[i]);
        }

        if (!recursive)
            return;

        string[] subDirs = Directory.GetDirectories(folderPath);
        Array.Sort(subDirs, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < subDirs.Length; i++)
        {
            var folderName = Path.GetFileName(subDirs[i]);
            if (folderName.Length == 0 || folderName[0] == '~' || folderName[0] == '!' || folderName[0] == '#')
                continue;

            CollectExcelFilesInternal(subDirs[i], true, result);
        }
    }


    private static List<ContainerFieldInfo> GetContainerFields<T>(T container) where T : class
    {
        return container.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f => new ContainerFieldInfo
            {
                Field = f,
                Binding = f.GetCustomAttribute<SheetBindingAttribute>()
            })
            .ToList();
    }

    private static void ValidateContainerFields<T>(T container, List<ContainerFieldInfo> containerFields) where T : class
    {
        foreach (var entry in containerFields)
        {
            object value = entry.Field.GetValue(container);
            bool isEmptyCollection = value is ICollection collection &&
                collection.Count == 0;
            if (entry.Binding != null &&
                !entry.Binding.optional &&
                (value == null || isEmptyCollection))
            {
                throw new Exception($"[ExcelLoader] Required sheet '{entry.Binding?.SheetName ?? entry.Field.Name}' not found for field '{entry.Field.Name}'");
            }
        }
    }

    private static void LoadExcel<T>(T container, string filePath, List<ContainerFieldInfo> containerFields) where T : class
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        bool isCsv = string.Equals(
            Path.GetExtension(filePath),
            ".csv",
            StringComparison.OrdinalIgnoreCase);
        using IExcelDataReader reader = isCsv
            ? ExcelReaderFactory.CreateCsvReader(stream)
            : ExcelReaderFactory.CreateReader(stream);
        var ds = reader.AsDataSet();
        if (isCsv && ds.Tables.Count == 1)
            ds.Tables[0].TableName = Path.GetFileNameWithoutExtension(filePath);

        foreach (DataTable sheet in ds.Tables)
        {
            string rawSheet = sheet.TableName?.Trim() ?? "";
            if (string.IsNullOrEmpty(rawSheet) || rawSheet.StartsWith("~") || rawSheet.StartsWith("#")) continue;

            bool isColumnBased = rawSheet.StartsWith("!") || rawSheet.StartsWith("*");
            if (isColumnBased) rawSheet = rawSheet[1..];

            string sheetName = rawSheet.Split('#')[0].Trim();

            List<SheetSectionInfo> sections = FindSheetSections(sheet);
            var sectionBoundFields = new HashSet<FieldInfo>();
            for (int i = 0; i < sections.Count; i++)
            {
                SheetSectionInfo section = sections[i];
                ContainerFieldInfo entry = containerFields.FirstOrDefault(
                    candidate => IsSectionMatch(candidate, section.Name));
                if (entry == null)
                    continue;

                DataTable sectionSheet = CreateSectionTable(
                    sheet,
                    section,
                    sections);
                bool columnBased = section.IsColumnBased ||
                    entry.Binding?.isColumnBased == true;
                ParseSheetAndStore(
                    container,
                    sectionSheet,
                    entry.Field,
                    columnBased,
                    filePath);
                sectionBoundFields.Add(entry.Field);
            }

            foreach (ContainerFieldInfo entry in containerFields)
            {
                if (sectionBoundFields.Contains(entry.Field))
                    continue;

                string boundSheetName = entry.Binding?.SheetName ??
                    entry.Field.Name;
                if (!string.Equals(
                        boundSheetName,
                        sheetName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool columnBased = isColumnBased ||
                    entry.Binding?.isColumnBased == true;
                ParseSheetAndStore(
                    container,
                    sheet,
                    entry.Field,
                    columnBased,
                    filePath);
            }
        }
    }

    private static List<SheetSectionInfo> FindSheetSections(DataTable sheet)
    {
        var sections = new List<SheetSectionInfo>();
        for (int row = 0; row < sheet.Rows.Count; row++)
        {
            for (int column = 0; column < sheet.Columns.Count; column++)
            {
                string cell = GetCellString(sheet.Rows[row][column]).Trim();
                Match match = SheetSectionRegex.Match(cell);
                if (!match.Success)
                    continue;

                string mode = match.Groups["mode"].Value;
                sections.Add(new SheetSectionInfo
                {
                    Name = match.Groups["name"].Value.Trim(),
                    Row = row,
                    Column = column,
                    IsColumnBased = mode == "*",
                });
            }
        }

        return sections;
    }

    private static bool IsSectionMatch(
        ContainerFieldInfo entry,
        string sectionName)
    {
        string boundSheetName = entry.Binding?.SheetName;
        Type dataType = GetDataType(entry.Field);
        return string.Equals(
                entry.Field.Name,
                sectionName,
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                boundSheetName,
                sectionName,
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                dataType.Name,
                sectionName,
                StringComparison.OrdinalIgnoreCase);
    }

    private static DataTable CreateSectionTable(
        DataTable source,
        SheetSectionInfo section,
        List<SheetSectionInfo> sections)
    {
        int startRow = section.Row + 1;
        int endRow = source.Rows.Count;
        int startColumn = section.Column;
        int endColumn = source.Columns.Count;

        for (int i = 0; i < sections.Count; i++)
        {
            SheetSectionInfo other = sections[i];
            if (other == section)
                continue;
            if (other.Column == section.Column && other.Row > section.Row)
                endRow = Math.Min(endRow, other.Row);
            if (other.Row == section.Row && other.Column > section.Column)
                endColumn = Math.Min(endColumn, other.Column);
        }

        var result = new DataTable(source.TableName);
        for (int column = startColumn; column < endColumn; column++)
            result.Columns.Add();

        for (int row = startRow; row < endRow; row++)
        {
            DataRow resultRow = result.NewRow();
            for (int column = startColumn; column < endColumn; column++)
                resultRow[column - startColumn] = source.Rows[row][column];
            result.Rows.Add(resultRow);
        }

        return result;
    }

    private static void ParseSheetAndStore(object container, DataTable sheet, FieldInfo field, bool isColumnBased, string filePath)
    {
        var dataList = ParseSheet(sheet, isColumnBased);
        StoreInContainer(container, sheet, field, dataList, filePath);
    }

    private static List<Dictionary<string, List<string>>> ParseSheet(DataTable sheet, bool isColumnBased)
    {
        var dataList = new List<Dictionary<string, List<string>>>();

        int primaryCount = isColumnBased ? sheet.Rows.Count : sheet.Columns.Count;
        int secondaryCount = isColumnBased ? sheet.Columns.Count : sheet.Rows.Count;

        if (primaryCount == 0 || secondaryCount <= 1)
        {
            Debug.LogWarning($"[ExcelLoader] Sheet {sheet.TableName} is empty or lacks enough {(isColumnBased ? "rows" : "columns")} for parsing.");
            return dataList;
        }

        int startIndex = 0;
        for (int i = 0; i < secondaryCount; i++)
        {
            string head = isColumnBased
                ? GetCellString(sheet.Rows[0][i])
                : GetCellString(sheet.Rows[i][0]);
            if (!string.IsNullOrEmpty(head) && (head.StartsWith("//") || head.StartsWith("##")))
            {
                startIndex++;
                continue;
            }
            break;
        }

        if (startIndex + 1 >= secondaryCount)
        {
            Debug.LogWarning($"[ExcelLoader] Sheet {sheet.TableName} is empty or lacks enough {(isColumnBased ? "rows" : "columns")} for parsing.");
            return dataList;
        }

        var headerMap = new Dictionary<int, string>();
        for (int i = 0; i < primaryCount; i++)
        {
            string head = isColumnBased
                ? GetCellString(sheet.Rows[i][startIndex])
                : GetCellString(sheet.Rows[startIndex][i]);
            if (!string.IsNullOrWhiteSpace(head)) headerMap[i] = head;
        }

        var grouped = headerMap
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value) && !kv.Value.StartsWith("~") && !kv.Value.StartsWith("#"))
            .GroupBy(
                kv => kv.Value.Split('#')[0].Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Select(kv => kv.Key).ToList(),
                StringComparer.OrdinalIgnoreCase);

        for (int j = startIndex + 1; j < secondaryCount; j++)
        {
            string head = isColumnBased
                ? GetCellString(sheet.Rows[0][j])
                : GetCellString(sheet.Rows[j][0]);
            if (!string.IsNullOrEmpty(head) && (head.StartsWith("//") || head.StartsWith("##"))) continue;

            var fieldValues = new Dictionary<string, List<string>>(
                grouped.Count,
                StringComparer.OrdinalIgnoreCase);
            bool hasData = false;

            foreach (var kv in grouped)
            {
                var values = new List<string>(kv.Value.Count);
                for (int index = 0; index < kv.Value.Count; index++)
                {
                    int cellIndex = kv.Value[index];
                    string value = isColumnBased
                        ? GetCellString(sheet.Rows[cellIndex][j])
                        : GetCellString(sheet.Rows[j][cellIndex]);
                    if (!string.IsNullOrWhiteSpace(value))
                        hasData = true;
                    values.Add(value);
                }
                fieldValues[kv.Key] = values;
            }

            if (!hasData) break;
            dataList.Add(fieldValues);
        }

        return dataList;
    }

    private static string GetCellString(object value)
    {
        if (value == null || value == DBNull.Value)
            return string.Empty;
        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture) ??
                string.Empty;
        return value.ToString() ?? string.Empty;
    }

    private static object ConvertAndValidate(List<string> cellStrList, FieldInfo field, DataTable sheet, string filePath, string rowIdentity)
    {
        ExcelParerAttribute excelParer = field.GetCustomAttribute<ExcelParerAttribute>();

        string cellStr = "";

        string separator = ",";

        if (excelParer != null)
        {
            separator = excelParer.Separator;
        }

        if (cellStrList != null && cellStrList.Count > 0)
        {
            cellStr = cellStrList[0];
        }

        if (string.IsNullOrWhiteSpace(cellStr))
        {
            return excelParer != null ? excelParer.DefaultValue : GetDefaultValue(field.FieldType);
        }

        if (excelParer != null && excelParer.MergedCells)
        {
            cellStr = string.Join(
                separator,
                cellStrList.Where(item => !string.IsNullOrEmpty(item)));
            cellStrList = new List<string> { cellStr };
        }

        object finalVal = null;
        try
        {
            if (field.FieldType.IsArray)
            {
                Type elemType = field.FieldType.GetElementType();

                if (cellStrList != null && cellStrList.Count > 1)
                {
                    var splitted = cellStrList.Select(s => TryParseCellStr(s?.Trim(), elemType, excelParer)).ToArray();
                    var arr = Array.CreateInstance(elemType, splitted.Length);
                    splitted.CopyTo(arr, 0);
                    finalVal = arr;
                }
                else
                {
                    var splitted = cellStr.Split(separator).Select(s => TryParseCellStr(s?.Trim(), elemType, excelParer)).ToArray();
                    var arr = Array.CreateInstance(elemType, splitted.Length);
                    splitted.CopyTo(arr, 0);
                    finalVal = arr;
                }


            }
            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elemType = field.FieldType.GetGenericArguments()[0];

                var listObj = Activator.CreateInstance(field.FieldType) as System.Collections.IList;

                if (cellStrList != null && cellStrList.Count > 1)
                {
                    foreach (var part in cellStrList)
                    {
                        string trimmed = part.Trim();
                        listObj.Add(TryParseCellStr(trimmed, elemType, excelParer));
                    }
                }
                else
                {
                    foreach (var part in cellStr.Split(separator))
                    {
                        string trimmed = part.Trim();
                        if (!string.IsNullOrEmpty(trimmed))
                            listObj.Add(TryParseCellStr(trimmed, elemType, excelParer));
                    }
                }


                finalVal = listObj;
            }
            else
            {
                finalVal = TryParseCellStr(cellStrList, field.FieldType, excelParer);
            }
        }
        catch (Exception ex)
        {
            if (excelParer?.DefaultValue != null)
                return excelParer.DefaultValue;

            throw new InvalidDataException(
                $"[ExcelLoader] Convert error in sheet '{sheet.TableName}' " +
                $"(File: '{filePath}', Row: {rowIdentity}): Field " +
                $"'{field.Name}' ({field.FieldType.Name}), Cell Value: " +
                $"'{cellStr}'.",
                ex);
        }

        // 범위 검증
        var rangeAttr = field.GetCustomAttribute<ValidateRangeAttribute>();
        if (rangeAttr != null)
        {
            double dVal;
            try
            {
                dVal = Convert.ToDouble(
                    finalVal,
                    CultureInfo.InvariantCulture);
            }
            catch (Exception exception)
            {
                throw new InvalidDataException(
                    $"[ExcelLoader] [{rowIdentity}] in Sheet '{sheet.TableName}' " +
                    $"(File: '{filePath}'): Field '{field.Name}' is not numeric.",
                    exception);
            }
            if (dVal < rangeAttr.Min || dVal > rangeAttr.Max)
                throw new Exception($"[ExcelLoader] [{rowIdentity}] in Sheet '{sheet.TableName}' (File: '{filePath}'): Field '{field.Name}'={dVal} out of range [{rangeAttr.Min},{rangeAttr.Max}]");
        }

        // 정규식 검증
        var regexAttr = field.GetCustomAttribute<ValidateRegexAttribute>();
        if (regexAttr != null)
        {
            string sVal = finalVal?.ToString() ?? "";
            if (!Regex.IsMatch(sVal, regexAttr.Pattern))
                throw new Exception($"[ExcelLoader] [{rowIdentity}] in Sheet '{sheet.TableName}' (File: '{filePath}'): Field '{field.Name}'='{sVal}' doesn't match pattern '{regexAttr.Pattern}'");
        }

        return finalVal;
    }

    private static object TryParseCellStr(string cellStr, Type type, ExcelParerAttribute excelParer = null)
    {
        if (string.IsNullOrEmpty(cellStr))
        {
            return excelParer != null ? excelParer.DefaultValue : GetDefaultValue(type);
        }

        cellStr = cellStr.Trim();
        Type valueType = Nullable.GetUnderlyingType(type) ?? type;

        if (excelParer != null && excelParer.CustomParser != null)
        {
            ICustomParser parser = (ICustomParser)Activator.CreateInstance(excelParer.CustomParser);
            return parser.Parse(cellStr);
        }

        var customParserValue = TryParseUsingStaticMethod(cellStr, valueType);
        if (customParserValue != null)
            return customParserValue;

        if (typeof(ICustomParser).IsAssignableFrom(valueType))
        {
            ICustomParser parser = (ICustomParser)Activator.CreateInstance(valueType);
            return parser?.Parse(cellStr);
        }

        if (valueType == typeof(bool))
        {
            if (cellStr == "0") return false;
            if (cellStr == "1") return true;
            if (bool.TryParse(cellStr, out bool boolResult)) return boolResult;

            throw new FormatException($"Cannot parse '{cellStr}' to Boolean");
        }

        if (valueType.IsEnum)
        {
            if (string.IsNullOrEmpty(cellStr))
            {
                return excelParer != null
                    ? excelParer.DefaultValue
                    : GetDefaultValue(valueType);
            }

            foreach (var fieldInfo in valueType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = fieldInfo.GetCustomAttribute<ExcelEnumAttribute>();
                if (attr != null && string.Equals(attr.Value, cellStr, StringComparison.OrdinalIgnoreCase))
                {
                    return fieldInfo.GetValue(null);
                }
            }

            if (Enum.TryParse(valueType, cellStr, true, out object enResult))
            {
                return enResult;
            }

            if (excelParer?.DefaultValue != null)
                return excelParer.DefaultValue;

            throw new FormatException(
                $"Cannot parse '{cellStr}' to enum type '{valueType.Name}'");
        }

        if (valueType == typeof(Vector2))
        {
            return Vector2Parser.ParseValue(cellStr);
        }

        if (valueType == typeof(Vector3))
        {
            return Vector3Parser.ParseValue(cellStr);
        }

        return Convert.ChangeType(
            cellStr,
            valueType,
            CultureInfo.InvariantCulture);
    }

    private static object TryParseCellStr(List<string> cellStrList, Type type, ExcelParerAttribute excelParer = null)
    {
        string separator = ",";

        string cellStr = "";

        if (excelParer != null)
        {
            separator = excelParer.Separator;
        }

        if (cellStrList != null && cellStrList.Count > 0)
        {
            cellStr = cellStrList[0];
        }

        if (excelParer != null && excelParer.MergedCells)
        {
            return string.Join(separator, cellStrList.Where(item => !string.IsNullOrEmpty(item)));
        }

        return TryParseCellStr(cellStr, type, excelParer);
    }

    private static object TryParseUsingStaticMethod(string value, Type targetType)
    {
        var method = targetType.GetMethod(
            "ParseValue",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new[] { typeof(string) },
            null);
        if (method != null)
            return method.Invoke(null, new object[] { value });
        return null;
    }

    private static object GetDefaultValue(Type t)
    {
        if (t == typeof(string)) return "";
        if (t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte) ||
            t == typeof(uint) || t == typeof(ulong) || t == typeof(ushort))
            return 0;
        if (t == typeof(float)) return 0f;
        if (t == typeof(double)) return 0.0;
        if (t == typeof(decimal)) return 0m;
        if (t == typeof(bool)) return false;
        if (t == typeof(Vector2)) return Vector2.zero;
        if (t == typeof(Vector3)) return Vector3.zero;
        if (t.IsValueType) return Activator.CreateInstance(t);
        return null;
    }

    private static void StoreInContainer(object container, DataTable sheet, FieldInfo parentField, List<Dictionary<string, List<string>>> dataList, string filePath)
    {
        Type dataType = GetDataType(parentField);

        if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(List<>))
        {
            dataType = dataType.GetGenericArguments()[0];

        }
        else if (dataType.IsArray)
        {
            dataType = dataType.GetElementType();
        }

        var bindAttr = parentField.GetCustomAttribute<SheetBindingAttribute>();

        List<DataFieldInfo> dataFields = dataType
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(field =>
            {
                ExcelParerAttribute parser =
                    field.GetCustomAttribute<ExcelParerAttribute>();
                return new DataFieldInfo
                {
                    Field = field,
                    Parser = parser,
                    MultiParser =
                        field.GetCustomAttribute<MultiColumnParserAttribute>(),
                    ColumnName = string.IsNullOrEmpty(parser?.ColumnName)
                        ? field.Name
                        : parser.ColumnName,
                };
            })
            .Where(field => field.Parser == null || !field.Parser.Ignore)
            .ToList();
        var fieldsByColumn = new Dictionary<string, DataFieldInfo>(
            dataFields.Count,
            StringComparer.OrdinalIgnoreCase);
        foreach (DataFieldInfo dataField in dataFields)
        {
            if (!fieldsByColumn.TryAdd(dataField.ColumnName, dataField))
            {
                throw new InvalidDataException(
                    $"[ExcelLoader] Type '{dataType.Name}' maps multiple " +
                    $"fields to column '{dataField.ColumnName}'.");
            }

            if (dataField.Parser?.RequiredColumn == true &&
                dataList.Count > 0 &&
                !dataList[0].ContainsKey(dataField.ColumnName))
            {
                throw new InvalidDataException(
                    $"[ExcelLoader] Required column '{dataField.ColumnName}' " +
                    $"was not found in sheet '{sheet.TableName}' " +
                    $"(File: '{filePath}').");
            }
        }

        MethodInfo keyMethod = dataType.GetMethod(
            "Key",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);
        foreach (var data in dataList)
        {
            object instance = Activator.CreateInstance(dataType);
            object objectKey = null;
            string rowIdentity = GetRowIdentity(data);

            foreach (var kv in data)
            {
                if (!fieldsByColumn.TryGetValue(kv.Key, out DataFieldInfo fieldInfo))
                    continue;

                object fieldValue = ConvertAndValidate(
                    kv.Value,
                    fieldInfo.Field,
                    sheet,
                    filePath,
                    rowIdentity);
                objectKey ??= fieldValue;
                fieldInfo.Field.SetValue(instance, fieldValue);
            }

            foreach (DataFieldInfo dataField in dataFields)
            {
                MultiColumnParserAttribute mpAttr = dataField.MultiParser;
                if (mpAttr == null || mpAttr.ColumnNames == null || mpAttr.ColumnNames.Length == 0)
                    continue;

                bool isValid = mpAttr.ColumnNames.All(col => !string.IsNullOrWhiteSpace(col) && data.ContainsKey(col));
                if (!isValid) continue;

                var values = mpAttr.ColumnNames.Select(col => ((data[col] == null || data[col].Count == 0) ? "" : data[col][0])).ToArray();
                var mp = (IMultiColumnParser)Activator.CreateInstance(mpAttr.ParserType);
                dataField.Field.SetValue(instance, mp.Parse(values));
            }

            object key = keyMethod != null
                ? keyMethod.Invoke(instance, null)
                : objectKey;

            FillBoundField(container, parentField, dataType, key, instance, bindAttr, sheet, filePath, rowIdentity);
        }
    }

    private static Type GetDataType(FieldInfo field)
    {
        if (IsDictType(field.FieldType, out _, out var valType))
            return valType;
        else if (field.FieldType.IsArray)
            return field.FieldType.GetElementType();
        else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            return field.FieldType.GetGenericArguments()[0];
        return field.FieldType;
    }

    private static void FillBoundField(object container, FieldInfo field, Type dataType, object key, object dataItem, SheetBindingAttribute bindAttr, DataTable sheet, string filePath, string rowIdentity)
    {
        if (IsDictType(field.FieldType, out _, out var valType))
        {
            var dictVal = field.GetValue(container);
            if (dictVal == null)
            {
                dictVal = Activator.CreateInstance(field.FieldType);
                field.SetValue(container, dictVal);
            }
            var dictID = dictVal as IDictionary;
            if (dictID == null)
            {
                throw new InvalidDataException(
                    $"[ExcelLoader] Field '{field.Name}' is not a dictionary.");
            }

            if (key == null)
            {
                throw new InvalidDataException(
                    $"[ExcelLoader] Null key for field '{field.Name}' " +
                    $"(Sheet: '{sheet.TableName}', File: '{filePath}', " +
                    $"Row: {rowIdentity}).");
            }

            if (dictID.Contains(key))
            {
                object existingValue = dictID[key];
                if (existingValue != null &&
                    (existingValue is IList || existingValue.GetType().IsArray))
                {
                    if (existingValue is IList list)
                    {
                        list.Add(dataItem);
                    }
                    else if (existingValue.GetType().IsArray)
                    {
                        Array array = (Array)existingValue;
                        int len = array.Length;
                        Array newArray = Array.CreateInstance(valType.GetElementType(), len + 1);
                        Array.Copy(array, newArray, len);
                        newArray.SetValue(dataItem, len);
                        dictID[key] = newArray;
                    }
                }
                else
                {
                    if (bindAttr == null || !bindAttr.skipDuplicates)
                    {
                        throw new Exception($"[ExcelLoader] Duplicate key '{key}' found in dict field '{field.Name}' (Sheet: '{sheet.TableName}', File: '{filePath}')\nRow: {rowIdentity}");
                    }
                    else
                    {
                        dictID[key] = dataItem;
                    }
                }
            }
            else
            {
                if (valType.IsArray)
                {
                    Array newArray = Array.CreateInstance(valType.GetElementType(), 1);
                    newArray.SetValue(dataItem, 0);
                    dictID[key] = newArray;
                }
                else if (valType.IsGenericType && valType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = Activator.CreateInstance(valType) as IList;
                    list.Add(dataItem);
                    dictID[key] = list;
                }
                else
                {
                    dictID[key] = dataItem;
                }
            }
        }
        else if (field.FieldType == dataType)
        {
            field.SetValue(container, dataItem);
        }
        else if (IsListOfType(field.FieldType, dataType))
        {
            var listVal = field.GetValue(container) as IList;
            if (listVal == null)
            {
                listVal = Activator.CreateInstance(field.FieldType) as IList;
                field.SetValue(container, listVal);
            }
            listVal.Add(dataItem);
        }
        else if (field.FieldType.IsArray && field.FieldType.GetElementType() == dataType)
        {
            var existingArray = field.GetValue(container) as Array;
            int existingLength = existingArray != null ? existingArray.Length : 0;
            Array newArray = Array.CreateInstance(dataType, existingLength + 1);
            if (existingArray != null)
                Array.Copy(existingArray, newArray, existingLength);
            newArray.SetValue(dataItem, existingLength);
            field.SetValue(container, newArray);
        }
        else
        {
            Debug.LogWarning($"[ExcelLoader] sheet '{sheet.TableName}' field '{field.Name}' has [SheetBinding({dataType.Name})], but type mismatch? {field.FieldType} in file '{filePath}'");
        }
    }

    private static bool IsListOfType(Type t, Type elem)
    {
        return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>) && t.GetGenericArguments()[0] == elem;
    }

    private static bool IsDictType(Type t, out Type keyType, out Type valType)
    {
        keyType = null;
        valType = null;
        if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            return false;
        var args = t.GetGenericArguments();
        keyType = args[0];
        valType = args[1];
        return true;
    }

    private static string GetRowIdentity(Dictionary<string, List<string>> data)
    {
        if (data == null) return "Unknown";
        
        var keys = new[]
        {
            "UID",
            "ID",
            "category",
            "CHAPTER",
            "Key",
            "Index",
            "Name",
        };
        foreach (var keyCandidate in keys)
        {
            var matchedKey = data.Keys.FirstOrDefault(k => string.Equals(k, keyCandidate, StringComparison.OrdinalIgnoreCase));
            if (matchedKey != null && data[matchedKey] != null && data[matchedKey].Count > 0 && !string.IsNullOrWhiteSpace(data[matchedKey][0]))
            {
                return $"{matchedKey}='{data[matchedKey][0]}'";
            }
        }
        
        var nonActive = data
            .Where(kv => kv.Value != null && kv.Value.Any(v => !string.IsNullOrWhiteSpace(v)))
            .Select(kv => $"{kv.Key}='{kv.Value[0]}'");
        if (nonActive.Any())
        {
            return string.Join(", ", nonActive);
        }
        
        return "EmptyRow";
    }
}

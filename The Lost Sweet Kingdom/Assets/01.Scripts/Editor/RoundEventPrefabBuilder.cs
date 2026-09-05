#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class RoundEventPrefabBuilder
{
    public const string PrefabPath = "Assets/Resources/Prefabs/Events/RoundEventPanel.prefab";

    [MenuItem("Tools/Events/Create Event UI Prefab")]
    public static void Create()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Debug.Log("기존 이벤트 프리팹을 선택했습니다. Inspector에서 편집할 수 있습니다.");
            return;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(PrefabPath));
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/Fonts/NotoSansKR-Regular SDF.asset");
        if (font == null) throw new System.InvalidOperationException("한글 폰트를 찾을 수 없습니다.");
        var root = Rect("RoundEventPanel", null);
        try
        {
            var canvas = root.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            var scaler = root.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;
            root.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            var controller = root.gameObject.AddComponent<RoundEventPanel>();
            var dim = Rect("Dim", root); Stretch(dim);
            Image(dim, new Color(.09f,.04f,.16f,.78f), true);
            var shell = Rect("Panel", root); Size(shell,1200,860,0,0);
            Image(shell, Hex("F7EAF6"), true);
            var outline = shell.gameObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = Hex("BF83CC"); outline.effectDistance = new Vector2(4,-4);
            var panelGroup = shell.gameObject.AddComponent<CanvasGroup>();
            var title = Text("Title", shell, "달콤한 왕국의 만남", font, 38, Hex("513362"));
            Size(title.rectTransform,1100,65,0,345);
            title.alignment = TextAlignmentOptions.Center;

            var purple = Rect("PurplePanel", shell); Size(purple,1100,360,0,115);
            Image(purple, Hex("DDB8E8"), false);
            var content = Rect("ArtAndDescription", purple); Stretch(content);
            var contentGroup = content.gameObject.AddComponent<CanvasGroup>();
            var artRect = Rect("Art", content); Size(artRect,470,310,-280,0);
            var art = Image(artRect,Color.white,false);
            art.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/03.Art/00.Scene/00.Title/TitleBackGround.png");
            art.preserveAspect = true;
            var desc = Text("Description",content,"이벤트 설명",font,28,Hex("3E294D"));
            Size(desc.rectTransform,500,295,255,0);
            desc.alignment = TextAlignmentOptions.MidlineLeft;

            var answers = Rect("Answers",shell); Size(answers,1100,270,0,-225);
            var layout = answers.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.spacing=12; layout.childControlWidth=true; layout.childControlHeight=true;
            layout.childForceExpandWidth=true; layout.childForceExpandHeight=true;
            var row = Rect("AnswerTemplate",answers);
            var element = row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
            element.minHeight=54; element.flexibleHeight=1;
            var answerComponent = row.gameObject.AddComponent<RoundEventAnswer>();
            var moving = Rect("Reveal",row); Stretch(moving);
            var group = moving.gameObject.AddComponent<CanvasGroup>();
            var answerButton = Rect("Button",moving); Stretch(answerButton);
            var background = Image(answerButton,Hex("EBAFD0"),true);
            var button = answerButton.gameObject.AddComponent<UnityEngine.UI.Button>();
            button.targetGraphic=background;
            var colors=button.colors; colors.highlightedColor=Hex("FFE3F2");
            colors.pressedColor=Hex("D48AB9"); colors.disabledColor=Hex("C6BEC8"); button.colors=colors;
            var label = Text("Label",answerButton,"선택지",font,27,Hex("42283E"));
            Stretch(label.rectTransform,24,8);
            label.alignment=TextAlignmentOptions.MidlineLeft;
            label.enableAutoSizing=true; label.fontSizeMin=22; label.fontSizeMax=27;
            Set(answerComponent,"group",group); Set(answerComponent,"button",button); Set(answerComponent,"label",label);
            row.gameObject.SetActive(false);

            var next = Rect("Continue",shell); Size(next,1100,74,0,-320);
            var nextImage=Image(next,Hex("EBAFD0"),true);
            var nextButton=next.gameObject.AddComponent<UnityEngine.UI.Button>(); nextButton.targetGraphic=nextImage;
            var nextText=Text("Label",next,"계속하기",font,28,Hex("42283E")); Stretch(nextText.rectTransform);
            nextText.alignment=TextAlignmentOptions.Center;
            next.gameObject.SetActive(false);
            var status = Text("Status",shell,"라운드의 만남",font,22,Hex("705B7C"));
            Size(status.rectTransform,1100,40,0,-390); status.alignment=TextAlignmentOptions.Center;
            Set(controller,"panelGroup",panelGroup); Set(controller,"contentGroup",contentGroup);
            Set(controller,"titleText",title); Set(controller,"descriptionText",desc); Set(controller,"statusText",status);
            Set(controller,"art",art); Set(controller,"answersRoot",answers); Set(controller,"answerTemplate",answerComponent);
            Set(controller,"continueButton",nextButton);
            PrefabUtility.SaveAsPrefabAsset(root.gameObject,PrefabPath);
            Debug.Log($"[Events] 프리팹 생성 완료: {PrefabPath}");
        }
        finally { Object.DestroyImmediate(root.gameObject); }
        AssetDatabase.SaveAssets();
    }

    internal static void Set(Object target,string property,Object value)
    {
        var serialized=new SerializedObject(target);
        serialized.FindProperty(property).objectReferenceValue=value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
    private static RectTransform Rect(string name,Transform parent)
    {
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        if(parent!=null) rect.SetParent(parent,false);
        return rect;
    }
    private static void Size(RectTransform rect,float width,float height,float x,float y)
    {
        rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);
        rect.sizeDelta=new Vector2(width,height); rect.anchoredPosition=new Vector2(x,y);
    }
    private static void Stretch(RectTransform rect,float horizontal=0,float vertical=0)
    {
        rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one;
        rect.offsetMin=new Vector2(horizontal,vertical); rect.offsetMax=new Vector2(-horizontal,-vertical);
    }
    private static UnityEngine.UI.Image Image(RectTransform rect,Color color,bool raycast)
    {
        var image=rect.gameObject.AddComponent<UnityEngine.UI.Image>(); image.color=color; image.raycastTarget=raycast;
        return image;
    }
    private static TextMeshProUGUI Text(string name,Transform parent,string value,TMP_FontAsset font,float size,Color color)
    {
        var text=Rect(name,parent).gameObject.AddComponent<TextMeshProUGUI>();
        text.font=font; text.text=value; text.fontSize=size; text.color=color; text.raycastTarget=false;
        text.textWrappingMode=TextWrappingModes.Normal;
        return text;
    }
    private static Color Hex(string value) { ColorUtility.TryParseHtmlString("#"+value,out var color); return color; }
}
#endif

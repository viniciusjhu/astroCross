using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class UIGenerator : EditorWindow
{
    [MenuItem("AstroCross/Gerar Menu UI")]
    public static void ShowWindow()
    {
        GetWindow<UIGenerator>("Gerador de UI");
    }

    // Configurações de Cores e Fontes
    private Color colorPrimary = new Color(0f, 0.878f, 1f); // #00E0FF (Ciano)
    private Color colorBgDark = new Color(0.058f, 0.09f, 0.137f, 0.3f); // #0F172A (Dark Blue) - 30% Opacity for Overlay
    private Color colorWhite = Color.white;
    
    private TMP_FontAsset mainFont; // Fonte Orbitron (o usuário precisará arrastar)

    void OnGUI()
    {
        GUILayout.Label("Configurações do Gerador", EditorStyles.boldLabel);
        mainFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Fonte (Orbitron)", mainFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Criar Menu Completo (Main Menu)"))
        {
            CreateCompleteMenu();
        }

        GUILayout.Space(10);
        GUILayout.Label("Gameplay (Fase)", EditorStyles.boldLabel);
        if (GUILayout.Button("Criar UI de Gameplay (HUD + Game Over)"))
        {
            CreateGameplayUI();
        }
    }

    void CreateGameplayUI()
    {
        // 1. Setup Canvas (Reutiliza lógica se não existir)
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas");
            Canvas c = canvasObj.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler cs = canvasObj.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Event System
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // --- HUD (Score In-Game) ---
        GameObject hudPanel = CreatePanel(canvasObj.transform, "HUD_Panel", true);
        // Remove imagem de fundo do HUD para ficar transparente
        if(hudPanel.GetComponent<Image>()) DestroyImmediate(hudPanel.GetComponent<Image>());
        
        GameObject scoreObj = FindOrCreateChild(hudPanel.transform, "ScoreText");
        TextMeshProUGUI scoreTmp = scoreObj.GetComponent<TextMeshProUGUI>();
        if (scoreTmp == null) scoreTmp = scoreObj.AddComponent<TextMeshProUGUI>();
        scoreTmp.text = "SCORE: 0";
        scoreTmp.fontSize = 50;
        scoreTmp.color = colorPrimary;
        scoreTmp.alignment = TextAlignmentOptions.Left;
        if (mainFont != null) scoreTmp.font = mainFont;
        
        RectTransform scoreRT = scoreObj.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(0, 1); // Top Left
        scoreRT.anchorMax = new Vector2(0, 1);
        scoreRT.pivot = new Vector2(0, 1);
        scoreRT.anchoredPosition = new Vector2(50, -50); // Margem
        scoreRT.sizeDelta = new Vector2(500, 100);

        // --- Game Over Panel ---
        GameObject gameOverPanel = CreatePanel(canvasObj.transform, "GameOver_Panel", false);
        
        // Fundo Escuro
        Image bgImg = gameOverPanel.GetComponent<Image>();
        if (bgImg == null) bgImg = gameOverPanel.AddComponent<Image>();
        bgImg.color = new Color(0.058f, 0.09f, 0.137f, 0.95f); // Quase opaco para esconder o jogo

        // Titulo
        CreateText(gameOverPanel.transform, "Title", "GAME OVER", 120, new Color(1f, 0.2f, 0.2f), new Vector2(0, 250)); // Vermelho
        
        // Score Final
        CreateText(gameOverPanel.transform, "FinalScore", "SCORE: 1234", 60, colorWhite, new Vector2(0, 100));

        // Botoes
        CreateButton(gameOverPanel.transform, "Btn_Restart", "TRY AGAIN", new Vector2(0, -50));
        CreateButton(gameOverPanel.transform, "Btn_Menu", "MAIN MENU", new Vector2(0, -150));

        // --- Setup Managers ---
        
        // 1. GameManager (Singleton) - Garante que existe na cena para iniciar
        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj == null) gmObj = new GameObject("GameManager");
        if (gmObj.GetComponent<GameManager>() == null) gmObj.AddComponent<GameManager>();

        // 2. GameplayUI Manager (Local Proxy) - Cuida dos botoes e referencias
        GameObject uiManagerObj = GameObject.Find("GameplayUI_Manager");
        if (uiManagerObj == null) uiManagerObj = new GameObject("GameplayUI_Manager");
        
        GameplayUIManager uiManager = uiManagerObj.GetComponent<GameplayUIManager>();
        if (uiManager == null) uiManager = uiManagerObj.AddComponent<GameplayUIManager>();

        // Linka as referencias no Proxy
        uiManager.scoreText = scoreTmp;
        uiManager.gameOverPanel = gameOverPanel;
        uiManager.finalScoreText = gameOverPanel.transform.Find("FinalScore").GetComponent<TextMeshProUGUI>();

        // Linka Botoes ao Proxy (que nao se destroi)
        LinkButton(gameOverPanel, "Btn_Restart", uiManager, "OnRestartClick");
        LinkButton(gameOverPanel, "Btn_Menu", uiManager, "OnMenuClick");

        Debug.Log("UI de Gameplay Gerada com Sucesso!");
    }

    void CreateCompleteMenu()
    {
        // 1. Setup Canvas
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas");
            Canvas c = canvasObj.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler cs = canvasObj.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Event System
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 2. Background (Raw Image)
        GameObject bg = FindOrCreateChild(canvasObj.transform, "Background_RawImage");
        RawImage rawImg = bg.GetComponent<RawImage>();
        if (rawImg == null) rawImg = bg.AddComponent<RawImage>();
        rawImg.color = Color.white; // Texture base (Full Opacity)
        StretchToFill(bg.GetComponent<RectTransform>());

        // 2.1 Overlay (Dark Tint)
        GameObject overlay = FindOrCreateChild(bg.transform, "Overlay");
        // Ensure RectTransform exists first
        if (overlay.GetComponent<RectTransform>() == null) overlay.AddComponent<RectTransform>();
        
        Image overlayImg = overlay.GetComponent<Image>();
        if (overlayImg == null) overlayImg = overlay.AddComponent<Image>();
        
        overlayImg.color = colorBgDark; // Use the variable (0.3f alpha)
        overlayImg.raycastTarget = false; 
        StretchToFill(overlay.GetComponent<RectTransform>());

        // 3. Main Menu Panel
        GameObject mainPanel = CreatePanel(canvasObj.transform, "MainMenu_Panel", true);
        
        // Título
        CreateText(mainPanel.transform, "Title", "ASTROCROSS", 100, colorPrimary, new Vector2(0, 300));
        CreateText(mainPanel.transform, "Subtitle", "A Journey Home", 40, new Color(1,1,1,0.9f), new Vector2(0, 220));

        // Botões
        CreateButton(mainPanel.transform, "Btn_Play", "PLAY", new Vector2(0, 50));
        CreateButton(mainPanel.transform, "Btn_Tutorial", "TUTORIAL", new Vector2(0, -50));
        CreateButton(mainPanel.transform, "Btn_Settings", "SETTINGS", new Vector2(0, -150));
        CreateButton(mainPanel.transform, "Btn_Quit", "QUIT", new Vector2(0, -250));

        // 4. Tutorial Panel
        GameObject tutorialPanel = CreatePanel(canvasObj.transform, "Tutorial_Panel", false);
        CreateText(tutorialPanel.transform, "Title", "TUTORIAL", 80, colorPrimary, new Vector2(0, 400));
        
        // Cards Container (Horizontal)
        GameObject cardsObj = new GameObject("Cards_Container");
        cardsObj.transform.SetParent(tutorialPanel.transform, false);
        RectTransform cardsRT = cardsObj.AddComponent<RectTransform>();
        cardsRT.sizeDelta = new Vector2(1200, 400);
        HorizontalLayoutGroup hlg = cardsObj.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 50;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childAlignment = TextAnchor.MiddleCenter;

        CreateCard(cardsObj.transform, "Card_Controls", "CONTROLS", "WASD / Arrows\nMove Freely");
        CreateCard(cardsObj.transform, "Card_Objective", "OBJECTIVE", "Move Forward (+Score)\nAvoid Obstacles");

        CreateButton(tutorialPanel.transform, "Btn_Back", "BACK", new Vector2(0, -400));

        // 5. Settings Panel
        GameObject settingsPanel = CreatePanel(canvasObj.transform, "Settings_Panel", false);
        CreateText(settingsPanel.transform, "Title", "SETTINGS", 80, colorPrimary, new Vector2(0, 400));
        
        // Volume Slider
        CreateSlider(settingsPanel.transform, "Volume_Slider", new Vector2(0, 50));
        CreateText(settingsPanel.transform, "VolumeLabel", "Master Volume", 30, Color.white, new Vector2(0, 100));

        CreateButton(settingsPanel.transform, "Btn_Back", "BACK", new Vector2(0, -400));

        // 6. Configurar Script Manager
        GameObject managerObj = GameObject.Find("MenuManager");
        if (managerObj == null) managerObj = new GameObject("MenuManager");
        MainMenuManager manager = managerObj.GetComponent<MainMenuManager>();
        if (manager == null) manager = managerObj.AddComponent<MainMenuManager>();

        manager.mainPanel = mainPanel;
        manager.tutorialPanel = tutorialPanel;
        manager.settingsPanel = settingsPanel;

        // Linkar botões (Tentativa básica)
        LinkButton(mainPanel, "Btn_Play", manager, "PlayGame");
        LinkButton(mainPanel, "Btn_Tutorial", manager, "ShowTutorial");
        LinkButton(mainPanel, "Btn_Settings", manager, "ShowSettings");
        LinkButton(mainPanel, "Btn_Quit", manager, "QuitGame");
        LinkButton(tutorialPanel, "Btn_Back", manager, "ShowMainPanel");
        LinkButton(settingsPanel, "Btn_Back", manager, "ShowMainPanel");

        Debug.Log("UI Gerada com Sucesso! Ajuste a fonte no Inspector se necessário.");
    }

    // --- Helpers ---

    GameObject CreatePanel(Transform parent, string name, bool active)
    {
        GameObject p = FindOrCreateChild(parent, name);
        if (p.GetComponent<RectTransform>() == null) p.AddComponent<RectTransform>();
        StretchToFill(p.GetComponent<RectTransform>());
        p.SetActive(active);
        return p;
    }

    void CreateText(Transform parent, string name, string content, float size, Color color, Vector2 pos)
    {
        GameObject tObj = FindOrCreateChild(parent, name);
        TextMeshProUGUI tmp = tObj.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = tObj.AddComponent<TextMeshProUGUI>();
        
        tmp.text = content;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        if (mainFont != null) tmp.font = mainFont;

        RectTransform rt = tObj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(800, 150);
    }

    void CreateButton(Transform parent, string name, string label, Vector2 pos)
    {
        GameObject bObj = FindOrCreateChild(parent, name);
        Image img = bObj.GetComponent<Image>();
        if (img == null) img = bObj.AddComponent<Image>();
        img.color = new Color(0, 0.878f, 1f, 0.1f); // Ciano transparente

        Button btn = bObj.GetComponent<Button>();
        if (btn == null) btn = bObj.AddComponent<Button>();
        
        // Outline
        if (bObj.GetComponent<Outline>() == null)
        {
            var outl = bObj.AddComponent<Outline>();
            outl.effectColor = colorPrimary;
            outl.effectDistance = new Vector2(2, -2);
        }

        RectTransform rt = bObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 60);
        rt.anchoredPosition = pos;

        // Label
        GameObject textObj = FindOrCreateChild(bObj.transform, "Text (TMP)");
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 30;
        tmp.color = colorPrimary;
        tmp.alignment = TextAlignmentOptions.Center;
        if (mainFont != null) tmp.font = mainFont;
        StretchToFill(textObj.GetComponent<RectTransform>());
    }

    void CreateCard(Transform parent, string name, string title, string desc)
    {
        GameObject card = new GameObject(name);
        card.transform.SetParent(parent, false);
        Image img = card.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);
        
        // Titulo do Card
        GameObject tObj = new GameObject("Title");
        tObj.transform.SetParent(card.transform, false);
        TextMeshProUGUI tTmp = tObj.AddComponent<TextMeshProUGUI>();
        tTmp.text = title;
        tTmp.fontSize = 24;
        tTmp.alignment = TextAlignmentOptions.Center;
        tTmp.color = colorPrimary;
        if (mainFont != null) tTmp.font = mainFont;
        tObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);

        // Desc
        GameObject dObj = new GameObject("Desc");
        dObj.transform.SetParent(card.transform, false);
        TextMeshProUGUI dTmp = dObj.AddComponent<TextMeshProUGUI>();
        dTmp.text = desc;
        dTmp.fontSize = 18;
        dTmp.alignment = TextAlignmentOptions.Center;
        if (mainFont != null) dTmp.font = mainFont;
        dObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
    }

    void CreateSlider(Transform parent, string name, Vector2 pos)
    {
        GameObject sObj = FindOrCreateChild(parent, name);
        Slider slider = sObj.GetComponent<Slider>();
        if (slider == null)
        {
            // Sliders complexos sao chatos de criar via codigo sem prefab, 
            // vamos criar uma estrutura basica
            slider = sObj.AddComponent<Slider>();
            if (sObj.GetComponent<RectTransform>() == null) sObj.AddComponent<RectTransform>();
            RectTransform rt = sObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 20);
            rt.anchoredPosition = pos;
            
            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(sObj.transform, false);
            bg.AddComponent<RectTransform>(); // Ensure RectTransform exists
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = Color.gray;
            StretchToFill(bg.GetComponent<RectTransform>());
            slider.targetGraphic = bgImg;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sObj.transform, false);
            fillArea.AddComponent<RectTransform>(); // Ensure RectTransform exists
            StretchToFill(fillArea.GetComponent<RectTransform>());
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            fill.AddComponent<RectTransform>(); // Ensure RectTransform exists
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = colorPrimary;
            StretchToFill(fill.GetComponent<RectTransform>());
            slider.fillRect = fill.GetComponent<RectTransform>();
        }
    }

    void LinkButton(GameObject panel, string btnName, MainMenuManager manager, string methodName)
    {
        Transform t = panel.transform.Find(btnName);
        if (t != null)
        {
            Button b = t.GetComponent<Button>();
            // Limpa listeners antigos para nao duplicar
            b.onClick.RemoveAllListeners();
            
            UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick, 
                (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        }
    }

    // Overload para GameManager (Fix para o erro de compilacao)
    void LinkButton(GameObject panel, string btnName, GameManager manager, string methodName)
    {
        Transform t = panel.transform.Find(btnName);
        if (t != null)
        {
            Button b = t.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            
            UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick, 
                (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        }
    }

    // Overload para GameplayUIManager (Proxy)
    void LinkButton(GameObject panel, string btnName, GameplayUIManager manager, string methodName)
    {
        Transform t = panel.transform.Find(btnName);
        if (t != null)
        {
            Button b = t.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            
            UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick, 
                (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        }
    }

    GameObject FindOrCreateChild(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        if (t == null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }
        return t.gameObject;
    }

    void StretchToFill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

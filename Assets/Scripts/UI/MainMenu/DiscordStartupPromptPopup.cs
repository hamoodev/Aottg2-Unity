using ApplicationManagers;
using Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    class DiscordStartupPromptPopup: BasePopup
    {
        // TODO: Define Locales for title
        protected override string Title => "Discord Startup Prompt";
        protected override float Width => 700f;
        protected override float Height => 300f;
        protected override float VerticalSpacing => 20f;
        protected override int VerticalPadding => 20;
        protected override bool UseSound => true;

        public override void Setup(BasePanel parent = null)
        {
            base.Setup(parent);
            string cat = "MainMenu";
            string sub = "ToolsPopup";
            float width = 220f;
            ElementStyle style = new ElementStyle(fontSize: ButtonFontSize, themePanel: ThemePanel);
            // ElementFactory.CreateTextButton(BottomBar, style, UIManager.GetLocaleCommon("Back"), onClick: () => OnButtonClick("Back"));
            // ElementFactory.CreateTextButton(SinglePanel, style, UIManager.GetLocale(cat, sub, "MapEditorButton"), width, onClick: () => OnButtonClick("MapEditor"));
            // ElementFactory.CreateTextButton(SinglePanel, style, UIManager.GetLocale(cat, sub, "CharacterEditorButton"), width, onClick: () => OnButtonClick("CharacterEditor"));
            // ElementFactory.CreateTextButton(SinglePanel, style, UIManager.GetLocale(cat, sub, "SnapshotViewerButton"), width, onClick: () => OnButtonClick("SnapshotViewer"));
            // ElementFactory.CreateTextButton(SinglePanel, style, UIManager.GetLocale(cat, sub, "GalleryButton"), width, onClick: () => OnButtonClick("Gallery"));
            ElementFactory.CreateDefaultLabel(SinglePanel, style, "Do you want to login with Discord?");
            ElementFactory.CreateTextButton(BottomBar, style, "No", onClick: () => OnButtonClick("No"));
            ElementFactory.CreateTextButton(BottomBar, style, "Yes", onClick: () => OnButtonClick("Yes"));
        }

        protected void OnButtonClick(string name)
        {
            if (name == "Yes")
            {
                Debug.Log("Yes button clicked - Starting Discord OAuth");
                DiscordManager.StartOAuth();
                Hide();
            }
            else if (name == "No")
            {
                Debug.Log("No button clicked - Skipping Discord OAuth");
                Hide();
            }
        }
    }
}

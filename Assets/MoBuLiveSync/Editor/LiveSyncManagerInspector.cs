using System.Net.NetworkInformation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MoBuLiveSync
{
    [CustomEditor(typeof(MoBuLiveSyncManager))]
    public class LiveSyncManagerInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var liveSyncManager = target as MoBuLiveSyncManager;
            var root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            if (liveSyncManager == null) return root;

            // Information Panel
            var infoPanel = new VisualElement();
            infoPanel.AddToClassList("info-panel");
            var label = new Label("MotionBuilder LiveSync Tool");
            label.AddToClassList("info-title");
            infoPanel.Add(label);
            var versionLabel = new Label($"Version: {MoBuLiveSyncManager.Version}");
            versionLabel.AddToClassList("info-subtitle");
            infoPanel.Add(versionLabel);
            var addressLabel = new Label($"Address: {GetAddress()}");
            infoPanel.Add(addressLabel);
            var portLabel = new Label($"Port: {liveSyncManager.ReceivingPort}");
            portLabel.AddToClassList("pb-3");
            infoPanel.Add(portLabel);

            var lastReceivedBytesText = new IntegerField
            {
                label = "Received Size (bytes)",
                bindingPath = "_lastReceiveSize",
                isReadOnly = true
            };
            infoPanel.Add(lastReceivedBytesText);
            var receiveRate = new IntegerField
            {
                label = "Receive Rate (req/sec)",
                bindingPath = "_receiveRate",
                isReadOnly = true
            };
            infoPanel.Add(receiveRate);
            root.Add(infoPanel);

            // Character Panel
            var characterTitleText = new Label("[Characters]");
            characterTitleText.AddToClassList("subtitle");
            root.Add(characterTitleText);

            var characterPanel = new VisualElement();
            characterPanel.AddToClassList("character-panel");
            var characterSubjectNameFoldout = new Foldout
            {
                text = "Synced Subject Names",
                value = true
            };
            characterSubjectNameFoldout.AddToClassList("subject-name-foldout");
            characterPanel.Add(characterSubjectNameFoldout);

            var characterSubjectNames = liveSyncManager.CharacterSubjectNames;
            var characterList = new ListView
            {
                selectionType = SelectionType.None,
                horizontalScrollingEnabled = false
            };
            characterList.AddToClassList("subject-list");
            characterList.makeItem = () =>
            {
                var characterElement = new VisualElement();
                characterElement.AddToClassList("subject-element");
                var characterName = new TextField()
                {
                    isReadOnly = true
                };
                characterName.AddToClassList("subject-name");
                characterElement.Add(characterName);
                var copyButton = new Button(() => { EditorGUIUtility.systemCopyBuffer = characterName.value; });
                copyButton.AddToClassList("subject-copy-button");
                copyButton.text = "Copy";
                characterElement.Add(copyButton);

                return characterElement;
            };
            characterList.bindItem = (element, index) =>
            {
                var character = characterSubjectNames[index];
                var characterName = element.Q<TextField>("", "subject-name");
                characterName.value = character;
            };
            characterList.itemsSource = characterSubjectNames;
            characterSubjectNameFoldout.Add(characterList);
            root.Add(characterPanel);

            // Prop Panel
            var propTitleText = new Label("[Props]");
            propTitleText.AddToClassList("subtitle");
            root.Add(propTitleText);

            var propPanel = new VisualElement();
            propPanel.AddToClassList("prop-panel");
            var propSubjectNameFoldout = new Foldout
            {
                text = "Synced Subject Names",
                value = true
            };
            propSubjectNameFoldout.AddToClassList("subject-name-foldout");
            propPanel.Add(propSubjectNameFoldout);

            var propSubjectNames = liveSyncManager.PropSubjectNames;
            var propList = new ListView
            {
                selectionType = SelectionType.None,
                horizontalScrollingEnabled = false
            };
            propList.AddToClassList("subject-list");
            propList.makeItem = () =>
            {
                var propElement = new VisualElement();
                propElement.AddToClassList("subject-element");
                var propName = new TextField()
                {
                    isReadOnly = true
                };
                propName.AddToClassList("subject-name");
                propElement.Add(propName);
                var copyButton = new Button(() => { EditorGUIUtility.systemCopyBuffer = propName.value; });
                copyButton.AddToClassList("subject-copy-button");
                copyButton.text = "Copy";
                propElement.Add(copyButton);

                return propElement;
            };
            propList.bindItem = (element, index) =>
            {
                var prop = propSubjectNames[index];
                var propName = element.Q<TextField>("", "subject-name");
                propName.value = prop;
            };
            propList.itemsSource = propSubjectNames;
            propSubjectNameFoldout.Add(propList);
            root.Add(propPanel);

            var styleSheet = Resources.Load<StyleSheet>("LiveSyncStyle");
            root.styleSheets.Add(styleSheet);
            return root;
        }


        private static string GetAddress()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in nics)
            {
                if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                var properties = adapter.GetIPProperties();
                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                    return address.Address.ToString();
                }
            }

            return string.Empty;
        }
    }
}

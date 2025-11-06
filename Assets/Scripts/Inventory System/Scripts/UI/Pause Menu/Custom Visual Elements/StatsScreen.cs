using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


namespace DreamersInc.MoonShot.GameCode.UI
{
    public class StatsScreen : VisualElement
    {
        public EquipmentView EquipmentView;
        public StatsView StatsView;

        public StatsScreen()
        {
            this.style.flexDirection = FlexDirection.Row;
            EquipmentView = new EquipmentView();
            EquipmentView.AddTo(this);
            StatsView = this.CreateChild<StatsView>();
        }

        public IEnumerator Generate(VisualElement parent)
        {
            parent.CreateChild<StatsScreen>();
            yield return null;
        }
    }

    public class StatsView : VisualElement
    {
        private Image characterIcon;

        Label characterName;
        Label characterStat;
        Label characterStatValue;

        public StatsView()
        {
            this.AddClass("StatsView");
            this.style.flexDirection = FlexDirection.Row;
            var container1 = this.CreateChild();
            characterName = container1.CreateChild<Label>("name");
            characterName.text = "player name";
            var statsContainer = container1.CreateChild();
            statsContainer.style.flexDirection = FlexDirection.Row;
            statsContainer.style.paddingLeft = 5;
            statsContainer.style.paddingRight = 5;
            characterStat = statsContainer.CreateChild<Label>("stat");
            characterStat.text = "player stats";
            characterStatValue = statsContainer.CreateChild<Label>("stat");
            characterStatValue.text = "player stats";
            characterIcon = this.CreateChild<Image>("characterIcon");
        }

        public void BindCharacter(StatsViewModel character)
        {
            characterName.dataSource = character;
            characterName.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(StatsViewModel.PlayerName)),
                bindingMode = BindingMode.ToTarget
            });
            characterStat.dataSource = character;
            characterStat.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(StatsViewModel.PlayerStats)),
                bindingMode = BindingMode.ToTarget
            });
            characterStatValue.dataSource = character;
            characterStatValue.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(StatsViewModel.PlayerStatsValues)),
                bindingMode = BindingMode.ToTarget
            });
            characterIcon.dataSource = character;
            characterIcon.SetBinding(nameof(Image.image), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(StatsViewModel.ActiveCharacterImage)),
                bindingMode = BindingMode.ToTarget
            });
        }
    }
}
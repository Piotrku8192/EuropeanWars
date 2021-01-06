using EuropeanWars.Core.Persons;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class ChoosePersonWindowElement : PersonButton {
        public ChoosePersonWindow window;
        public Text _moreInfo;

        public override void SetPerson(Person person) {
            base.SetPerson(person);
            _moreInfo.text = base.moreInfo.text;
        }

        public void Select() {
            window.SelectElement(this);
        }
    }
}

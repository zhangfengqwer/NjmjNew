namespace ETModel
{
    [Event(EventIdType.LoadRes)]
    public class LoadResEvent_CreateLoadResUI : AEvent
    {
        public override void Run()
        {
			Game.Scene.GetComponent<UIComponent>().Create(UIType.UILoadRes);
        }
    }
}

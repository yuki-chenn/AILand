namespace AILand.GamePlay.World.Prop
{
    public class Boat : BaseProp
    {
        public override PropType PropType => PropType.Boat;

        public override void Interact()
        {
            // TODO : 提交材料
            
            // 上船
            GameManager.Instance.GetOnBoard(gameObject);
        }
    }
}
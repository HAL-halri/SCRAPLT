[System.Serializable]
public class DropRateData
{
    //確率テーブルは変えずに新要素だけ追加する可能性があるため、stageも残しておく
    public int stage;
    public ItemData item;
    public float probability;
}

public interface ICell
{
    public void Activation();

    public void Deactivation();

    public void RestoreDefault();

    public void Reference();

    public void SetParentPool(CellPool pool);

    public void SetIndex(int index);
}
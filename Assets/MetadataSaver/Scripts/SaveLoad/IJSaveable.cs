namespace SaveLoad
{
	public interface IJSaveable
	{
		public void PrepareToSaveObjectState(MetaData metaData);

		public void LoadObjectState(MetaData metaData);
	}
}
namespace Smart.Essence
{
	public interface IFixedUpdatable
	{
		void FixedUpdate();
		bool IsActive { get; }
	}
}
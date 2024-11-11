namespace Smart.Essence
{
	public interface IUpdatable
	{
		void Update(float deltaTime);
		bool IsActive { get; }
	}
}
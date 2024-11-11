namespace Smart.Essence
{
	public interface ILateUpdatable
	{
		void LateUpdate(float deltaTime);
		bool IsActive { get; }
	}
}
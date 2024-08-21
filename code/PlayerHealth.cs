using Sandbox;

public sealed class PlayerHealth : Component
{
	[Property] [Sync] public int Percentage { get; set; } = 1;
	[Property] [Sync] public int Life { get; set; } = 999;

	[Broadcast]
	public void GetHurt(int AddPerc, Vector3 ouch)
	{
		this.Components.Get<CharacterController>().Punch( ouch * Percentage );
		Percentage += AddPerc;
	}
}

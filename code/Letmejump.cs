using Sandbox;

public sealed class Letmejump : Component
{
	public CharacterController ChrCon;
	protected override void OnAwake()
	{
		ChrCon = Components.Get<CharacterController>();

	}
	protected override void OnUpdate()
	{
		if ( Input.Pressed( "Jump" ) ) 
		{
			ChrCon.Punch( Vector3.Up * 300 );
			Log.Warning( "Let me jump" );

		}
	}
}

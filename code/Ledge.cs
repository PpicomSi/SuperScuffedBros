using Sandbox;
using Sandbox.Citizen;
using System.Diagnostics;

public sealed class Ledge : Component, Component.ITriggerListener
{
	[Property] GameObject Parent;
	[Property] float Offset;
	[Property] bool IsLeft;

	[Property] GameObject HandIK1;
	[Property] GameObject HandIK2;
	public void OnTriggerEnter(Collider other )
	{
		var player = other.Components.Get<PlayerMovement>();
		var playeranm = other.Components.Get<CitizenAnimationHelper>();
		if ( player != null )
		{
			player.IsLedging = true;
			playeranm.IkLeftHand = HandIK2;
			playeranm.IkRightHand = HandIK1;
			if ( IsLeft )
			{
				player.IsLedgeLeft = true;
				player.Transform.LerpTo( new Transform( Parent.Transform.LocalPosition - new Vector3( 0, 0, Offset )),1f);
				player.Model.Transform.Rotation = Rotation.Lerp( player.Model.Transform.Rotation , new Angles( 0, -90, 0 ), 1f);

			}
			else
			{
				player.IsLedgeLeft = false;

				player.Transform.LerpTo( new Transform( Parent.Transform.LocalPosition - new Vector3( 0, 0, Offset )),1f);
				player.Model.Transform.Rotation = Rotation.Lerp( player.Model.Transform.Rotation, new Angles( 0, -270, 0 ), 1f );

			}
		}
	}
	public void OnTriggerExit(Collider other)
	{
		var player = other.Components.Get<PlayerMovement>();
		var playeranm = other.Components.Get<CitizenAnimationHelper>();

		if ( player != null )
		{
			player.IsLedging = false;
			playeranm.IkLeftHand = null;
			playeranm.IkRightHand = null;
		}
	}
}

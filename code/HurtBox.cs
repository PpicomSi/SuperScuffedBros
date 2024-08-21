using Sandbox;
using Sandbox.Citizen;
using System.Diagnostics;

public sealed class HurtBox : Component, Component.ITriggerListener, Component.ICollisionListener
{
	[Property] SphereCollider SphereCollider;
	[Property] ModelRenderer Sphere;
	[Property] Vector3 position;

	[Broadcast]
	public void HurtStart()
	{
		if(!SphereCollider.Enabled)
		{
			this.Transform.LocalPosition = position;
			SphereCollider.Enabled = true;
			Sphere.Enabled = true;
		}
	}

	[Broadcast]
	public void HurtStop()
	{
		if ( SphereCollider.Enabled )
		{
			SphereCollider.Enabled = false;
			Sphere.Enabled = false;
		}
	}

	public void OnTriggerEnter( Collider other )
	{
		if (other == GameObject.Parent.Parent.Components.Get<BoxCollider>()) return;
		other.Components.Get<PlayerHealth>().GetHurt( 30, (other.Transform.Position - this.GameObject.Parent.Parent.Transform.Position).Normal + new Vector3(1,1,3) );
	}

	public void OnTriggerExit( Collider other )
	{

	}
}

using Sandbox;
using Sandbox.Citizen;
using Sandbox.VR;
using System.Numerics;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

public sealed class PlayerMovement : Component
{
	public float GravityMultiply = 3f;

	[Property] public float GroundControl { get; set; } = 5.0f;

	[Property] public float AirControl { get; set; } = 4.0f;

	[Property] public float MaxForce { get; set; } = 50f;

	[Property] public float Speed { get; set; } = 180f;

	[Property] public float JumpForce { get; set; } = 500f;

	[Property] public float CrouchSpeed { get; set; } = 100f;

	[Property] public GameObject Model { get; set; }

	[Property] public int MaxJump { get; set; } = 2;

	public int JumpLeft;

	public Vector3 WishVelocity = Vector3.Zero;

	[Property] HurtBox Hurtt;

	[Sync] public bool IsCrouching { get; set; } = false;

	private CharacterController ChrCon;
	private CitizenAnimationHelper ChrAnm;

	[Sync] public bool IsLedging { get; set; } = false;

	[Sync] public Angles Angles { get; set; } = new Angles(0, -90, 0);

	[Sync] public bool IsLedgeLeft { get; set; }
	protected override void OnAwake()
	{
		ChrCon = Components.Get<CharacterController>();
		ChrAnm = Components.Get<CitizenAnimationHelper>();
		JumpLeft = MaxJump;
	}


	protected override void OnFixedUpdate()
	{
		this.Transform.Position *= new Vector3( 0, 1, 1 );

		if ( IsLedging ) 
		{
			if ( Input.Pressed( "Forward" ) ) ExitLedge();
		}
		else
		{
			if ( !Network.IsProxy )
			{
				if ( Input.Down( "Backward" ) )
				{
					if ( ChrCon.IsOnGround ) IsCrouching = true;
					else GravityMultiply = 20;
				}
				else
				{
					IsCrouching = false;
					GravityMultiply = 3;
				}
				if ( Input.Pressed( "Forward" ) ) Jump();

				if ( Input.Down( "Left" ) )
				{
					Angles = Rotation.Lerp( Model.Transform.Rotation, new Angles( 0, -270, 0 ), Time.Delta * 20f );
				}
				else if ( Input.Down( "Right" ) )
				{
					Angles = Rotation.Lerp( Model.Transform.Rotation, new Angles( 0, -90, 0 ), Time.Delta * 20f );
				}
				BuildWishVelocity();
				Move();

				if ( Input.Pressed( "attack1" ) ) Attack();

			}


			Rotate();

			UpdateAnimation();

		};

	}

	async void Attack()
	{

		await Wait( 0.3f );

		
	}

	async Task Wait(float seconds)
	{
		Hurtt.HurtStart();

		await Task.DelaySeconds( seconds );

		Hurtt.HurtStop();
	}


	void BuildWishVelocity()
	{
		WishVelocity = 0;

		if ( Input.Down( "Right" ) ) WishVelocity -= new Vector3(0,1,0);
		if ( Input.Down( "Left" ) ) WishVelocity += new Vector3( 0, 1, 0 );

		WishVelocity = WishVelocity.WithZ( 0 );
		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( IsCrouching ) WishVelocity *= CrouchSpeed;
		else WishVelocity *= Speed;
	}

	void Move()
	{
		var gravity = Scene.PhysicsWorld.Gravity * GravityMultiply;

			if ( ChrCon.IsOnGround )
			{
				ChrCon.Velocity = ChrCon.Velocity.WithZ( 0 );
				ChrCon.Accelerate( WishVelocity );
				ChrCon.ApplyFriction( GroundControl );
			}
			else
			{
				//ChrCon.Velocity = gravity * Time.Delta * 0.5f;
				ChrCon.Accelerate( WishVelocity.ClampLength( MaxForce ) );
				ChrCon.ApplyFriction( AirControl );
			}

			ChrCon.Move();

			if ( !ChrCon.IsOnGround )
			{
				ChrCon.Velocity += gravity * Time.Delta;
			}
			else
			{
				ChrCon.Velocity = ChrCon.Velocity.WithZ( 0 );
			}


	}

	void UpdateAnimation()
	{
		if (ChrAnm is null) return;
		ChrAnm.WithWishVelocity( WishVelocity );
		ChrAnm.WithVelocity(ChrCon.Velocity );
		ChrAnm.IsGrounded = ChrCon.IsOnGround;
		ChrAnm.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		ChrAnm.DuckLevel = IsCrouching ? 1 : 0;
	}

	void Rotate()
	{
		if ( Model is null ) return;

		Model.Transform.Rotation = Angles;
	}

	void Jump()
	{
		if (JumpLeft>0)
		{
			ChrCon.IsOnGround = false;
			ChrCon.Velocity = new Vector3(0, 0, JumpForce);
			JumpAnim();
			ChrCon.Move();
			JumpLeft--;
		}

		if ( ChrCon.IsOnGround ) JumpLeft = MaxJump;
	}


	void ExitLedge()
	{
		var gravity = Scene.PhysicsWorld.Gravity * GravityMultiply;

		if ( IsLedging)
		{
			ChrCon.Move();

			ChrCon.Velocity += gravity * Time.Delta;


			if ( IsLedgeLeft )
			{
				if ( Input.Down( "Right" ) )
				{
					ChrCon.Transform.LerpTo( new Transform( ChrCon.Transform.LocalPosition + new Vector3( 0, -0.5f, 1 ) * 150 ), 1 );
				}
				else if ( Input.Down( "Left" ) )
				{
					ChrCon.Transform.LerpTo( new Transform( ChrCon.Transform.LocalPosition + new Vector3( 0, 0, 1.5f ) * 150 ), 1 );
				}
				else
				{
					ChrCon.Transform.LerpTo( new Transform( ChrCon.Transform.LocalPosition + new Vector3( 0, -0.5f, 1 ) * 150 ), 1 );
				}
			}
			else
			{
				if ( Input.Down( "Right" ) )
				{
					ChrCon.Transform.LerpTo( new Transform( ChrCon.Transform.LocalPosition + new Vector3( 0, 0, 1.5f ) * 150 ), 1 );
				}
				else if ( Input.Down( "Left" ) )
				{
					ChrCon.Transform.LerpTo( new Transform( ChrCon.Transform.LocalPosition + new Vector3( 0, 0.5f, 1 ) * 150 ), 1 );
				}
				else
				{
					ChrCon.Transform.LerpTo( new Transform( ChrCon.Transform.LocalPosition + new Vector3( 0, 0.5f, 1 ) * 150 ), 1 );
				}
			}

			JumpAnim();
			ChrCon.Move();

		}
	}

	[Broadcast]
	void JumpAnim()
	{
		ChrAnm?.TriggerJump();
	}
}

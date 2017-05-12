using System.Linq;
using UnityEngine;
using UnitySteer.Behaviors;

[RequireComponent(typeof (AutonomousVehicle))]
public class TagPlayer : MonoBehaviour
{
    public enum PlayerState
    {
        Neutral,
        Pursuer,
        Prey
    }

    private PlayerState _state = PlayerState.Neutral;

    private Material _baseMaterial;

    [SerializeField] private float _originalRadius = 0.3f;

    [SerializeField] private float _sizeVariance = 0.2f;

    [SerializeField] private Renderer _renderer;

    [SerializeField] private Material _glowMaterial;

    [SerializeField] private Material _preyMaterial;

    public AutonomousVehicle Vehicle { get; set; }
    public SteerForPursuit ForPursuit { get; private set; }
    public SteerForNeighborGroup ForNeighbors { get; private set; }
    public SteerForWander ForWander { get; private set; }
    public SteerForEvasion ForEvasion { get; private set; }

    public float OriginalSpeed { get; private set; }
    public float OriginalTurnTime { get; private set; }

    public PlayerState State
    {
        get { return _state; }
        set { SetState(value); }
    }

    public TrailRenderer Trail { get; private set; }

    public void ChangeSize(float percent)
    {
        Vehicle = GetComponent<AutonomousVehicle>();
        Vehicle.MaxSpeed *= 1 + percent;
        Vehicle.TurnTime *= 1 - (2 * percent);
        Vehicle.Transform.localScale *= 1 - (2 * percent);
        Vehicle.ScaleRadiusWithTransform(_originalRadius);

        OriginalSpeed = Vehicle.MaxSpeed;
        OriginalTurnTime = Vehicle.TurnTime;
    }

    public void Grow()
    {
        ChangeSize(-0.4f);
        _renderer.material = _glowMaterial;
        var tween = Go.to(transform, 0.3f, new GoTweenConfig().scale(2f, true).setIterations(8, GoLoopType.PingPong));
        tween.setOnCompleteHandler(x => _renderer.material = _baseMaterial);
    }

    public void Die()
    {
        transform.scaleTo(1.5f, 0.1f).setOnCompleteHandler(x => Destroy(gameObject));
    }

    private void Awake()
    {
        _sizeVariance = Mathf.Clamp(_sizeVariance, 0, 0.45f);

        var difference = Random.Range(-_sizeVariance, _sizeVariance);
        ChangeSize(difference);

        ForPursuit = GetComponent<SteerForPursuit>();
        ForNeighbors = GetComponent<SteerForNeighborGroup>();
        ForWander = GetComponent<SteerForWander>();
        ForEvasion = GetComponent<SteerForEvasion>();

        ForPursuit.OnArrival += OnReachPrey;

        Trail = GetComponent<TrailRenderer>();
        _baseMaterial = _renderer.material;
    }

    private void OnEnable()
    {
        TagPlayerManager.Instance.Players.Add(this);
    }

    private void OnDisable()
    {
        // Debug.Log(string.Format("{0} OnDisable called on {1}", Time.time, this));
        TagPlayerManager.Instance.Players.Remove(this);
    }


    private void SetState(PlayerState state)
    {
        Vehicle.MaxSpeed = OriginalSpeed;
        Vehicle.TurnTime = OriginalTurnTime;
        _state = state;
        switch (_state)
        {
            case PlayerState.Neutral:
                _renderer.material = _baseMaterial;
                _renderer.material.color = Color.white;
                break;
            case PlayerState.Prey:
                Vehicle.MaxSpeed *= 1.75f;
                Vehicle.TurnTime *= 0.95f;
                _renderer.material = _preyMaterial;
                _renderer.material.color = Color.yellow;
                break;
            case PlayerState.Pursuer:
                Vehicle.MaxSpeed *= 2f;
                _renderer.material.color = Color.red;
                break;
        }
        ForPursuit.enabled = State == PlayerState.Pursuer;
        ForWander.enabled = State == PlayerState.Neutral;
        ForNeighbors.enabled = State != PlayerState.Prey;
        ForEvasion.enabled = State == PlayerState.Prey;
        Trail.enabled = State == PlayerState.Prey;
    }

    // Update is called once per frame
    private void Update()
    {
        // Every frame, try to avoid the nearest attacker if we're the prey.
        if (State == PlayerState.Prey)
        {
            var closest =
                TagPlayerManager.Instance.Players.Where(x => x != this)
                    .OrderBy(x => (x.Vehicle.Position - Vehicle.Position).sqrMagnitude)
                    .First();
            ForEvasion.Menace = closest.Vehicle;
        }
    }

    private void OnReachPrey(Steering steering)
    {
        TagPlayerManager.Instance.CapturedPrey(this);
    }
}
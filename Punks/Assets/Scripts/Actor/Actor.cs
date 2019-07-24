using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]

public class Actor : MonoBehaviour, IDamagable
{
    [Header("Actor Data")]
    [SerializeField] private ActorData actorData;

    [SerializeField] private Territory territory;
    public Territory GetTerritory { get { return territory; } }

    [Header("Job")]
    [SerializeField] private Job job;

    [SerializeField] private Transform head;
    [SerializeField] private Transform torso;
    [SerializeField] private Transform legs;
    [SerializeField] private Transform shoes;

    [Header("Stats")]
    [SerializeField] private ActorStats currentStats;
    public ActorStats GetStats { get { return currentStats; } }

    [Header("Movement")]
    public float speed;
    public float maxVelocityChange = 10f;
    public float rotationSpeed = 10f;
    public float jumpForce = 10f;

    private new Rigidbody rigidbody;
    private Animator animator;
    private new Collider collider;

    [Header("Weapon")]
    [SerializeField]
    private Weapon defaultWeapon;
    [SerializeField]
    private Weapon _weapon;
    public Weapon weapon { get { return _weapon; } }
    private GameObject _weaponModel;
    [SerializeField]
    private Transform rightHand;
    private int attackNumber;
    [SerializeField]
    private float resetAttack = 2f;
    private float lastAttack;

    [Header("Dialogue")]
    [SerializeField] private DialogueTree _dialogue = new DialogueTree();
    public DialogueTree GetDialogue { get { return _dialogue; } }
    public void SetDialogue(DialogueTree dialogue)
    {
        _dialogue = dialogue;
    }

    //Damage Event
    public delegate void OnDamaged(Actor attacker);
    public event OnDamaged OnDamagedEvent;

    //Attacking Event
    public delegate void OnAttacking(Actor enemy);
    public event OnAttacking OnAttackingEvent;

    [Header("Ground Check Distance")]
    [SerializeField] float groundCheckDistance = .2f;

    //Flags
    [SerializeField]
    private bool _isGrounded;
    private bool _inJump;
    private bool _inAttack;
    private bool _isDead;
    public bool isDead { get { return _isDead; } }
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();

        LoadActor(actorData);
    }

    public void GenerateActor(Job job)
    {
        GenerateActor(job, null);
    }

    public void GenerateActor(Job job, Gang gang)
    {
        LoadActor(ActorFactory.GenerateActor(job, gang));
    }

    public void LoadActor(ActorData data)
    {
        actorData = data;
        Gang actorGang = GangManager.instance.GetGang(actorData.gang);

        if (actorGang)
            Customization.instance.DressCharacter(head, torso, legs, shoes, GetComponentInChildren<SkinnedMeshRenderer>(), actorData.appearance, actorGang.uniform);
        else
            Customization.instance.DressCharacter(head, torso, legs, shoes, GetComponentInChildren<SkinnedMeshRenderer>(), actorData.appearance);

        InitializeActorStats();
    }

    public ActorData GetActorData()
    {
        return actorData;
    }

    private void InitializeActorStats()
    {
        currentStats = actorData.stats;
    }

    private void Update()
    {
        if(weapon == null)
        {
            EquipWeapon(defaultWeapon);
        }

        if(currentStats.health <= 0 && !_isDead)
        {
            Kill();
        }

        _isGrounded = GroundCheck();

        if (_isGrounded && _inJump)
        {
            _inJump = false;
        }

        
        if(animator.GetCurrentAnimatorStateInfo(1).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(1).IsName("Empty"))
        {
            if (_inAttack && Time.time - lastAttack > 0.2f)
            {
                _inAttack = false;
            }

            if(Time.time - lastAttack >= resetAttack)
            {
                 ResetAttackNumber();
            }
        }

        if (string.IsNullOrEmpty(actorData.gang) == false)
        {
            if(GangManager.instance.GetGang(actorData.gang))
                UpdateCurrentTerritory();
        }
    }

    private void LateUpdate()
    {
        if(_isGrounded == false)
            animator.SetFloat("VerticalSpeed", rigidbody.velocity.y);
        else
            animator.SetFloat("VerticalSpeed", 0);
    }

    public void Move(Vector3 velocity)
    {
        Vector3 currentVelocity = rigidbody.velocity;
        Vector3 targetVelocity = velocity * speed;

        if(animator.GetCurrentAnimatorStateInfo(1).IsName("Idle") == false && animator.GetCurrentAnimatorStateInfo(1).IsName("Empty") == false)
            targetVelocity = Vector3.zero;

        Vector3 velocityChange = targetVelocity - currentVelocity;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        //Animation
        animator.SetFloat("MovementSpeed", velocity.magnitude);

        //Rotation
        RotateTowards(velocity);
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        Quaternion currentRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(direction, transform.up);

        transform.rotation = Quaternion.Slerp(currentRot, targetRot, rotationSpeed * Time.deltaTime);
    }

    public IEnumerator RotateOverTime(Vector3 direction, float time)
    {
        float timer = 0;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(direction, transform.up);

        while (timer < time)
        {
            timer += Time.deltaTime;

            transform.rotation = Quaternion.Slerp(startRot, targetRot, timer / time);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Jump()
    {
        if (_inJump || _isGrounded == false)
            return;

        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        animator.CrossFade("Jump", .1f);

        _inJump = true;
    }

    private bool GroundCheck()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, -Vector3.up, groundCheckDistance);
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (_weapon != null && _weapon.canBeDropped)
            DropWeapon();

        _weapon = Instantiate(weapon);
        if (weapon.model)
        {
            _weaponModel = Instantiate(weapon.model, rightHand);
            _weaponModel.transform.localPosition = weapon.offset;
            _weaponModel.transform.localRotation = Quaternion.Euler(weapon.rotation);
            _weaponModel.transform.localScale = weapon.scale;
        }
    }

    public void DropWeapon()
    {
        PickupManager.instance.CreatePickup(transform.position + transform.up + transform.forward, weapon);

        _weapon = Instantiate(defaultWeapon);

        if (_weaponModel)
            Destroy(_weaponModel);
    }

    public void UpdateHealth(int amount)
    {
        currentStats.health += amount;

        if (currentStats.health < 0)
            currentStats.health = 0;

    }

    public bool Damage(int damage, Actor attacker)
    {
        if (_isDead)
        return false;

        int defense = 0;

        for (int i = 0; i < currentStats.defense; i++)
        {
            int chance = Random.Range(0, 4);
            if (chance == 0)
                defense++;
        }

        int finalDmg = damage - defense;
        if (finalDmg < 0) finalDmg = 0;

        UpdateHealth(-finalDmg);

        Debug.Log("Damaged for::" + finalDmg);

        animator.CrossFade("Hit", .1f);

        if (OnDamagedEvent != null)
            OnDamagedEvent.Invoke(attacker);

        return true;
    }

    public void Attacking(Actor enemy)
    {
        if (OnAttackingEvent != null)
            OnAttackingEvent.Invoke(enemy);
    }

    private void Kill()
    {
        animator.CrossFade("Knockout Back", .1f);
        _isDead = true;
        actorData.hospitalizedDay = DayNightCycle.instance.dayNumber;
        actorData.hospitalizedDuration = Random.Range(4, 8);
    }

    public void MainAttack()
    {
        if (_inAttack)
            return;

        _weapon.MainAttack(this, attackNumber);

        lastAttack = Time.time;

        attackNumber++;
        attackNumber %= _weapon.mainAnimation.Length;
    }

    public void SecondaryAttack()
    {
        if (_inAttack)
            return;

        _weapon.SecondaryAttack(this, attackNumber);

        lastAttack = Time.time;

        attackNumber++;
        attackNumber %= _weapon.secondaryAnimation.Length;
    }

    public void AnimateAttack(string animationState)
    {
        _inAttack = true;
        animator.CrossFade(animationState, 0);
        
    }

    void ResetAttackNumber()
    {
        attackNumber = 0;
    }

    void UpdateCurrentTerritory()
    {
        Territory oldTerritory = territory;
        territory = TerritoryManager.instance.FindTerritory(transform.position);

        if (oldTerritory != null)
            oldTerritory.RemoveActor(actorData);

        if (territory != null)
            territory.AddActor(actorData);
    }

    public void ProgressAttack(float amount)
    {
        currentStats.attackProgress += amount;

        if (currentStats.attackProgress >= 1f)
        {
            currentStats.attack++;
            currentStats.attackProgress -= 1f;
        }
    }

}

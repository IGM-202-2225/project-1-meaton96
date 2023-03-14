using UnityEngine;

public class BombBehaviour : MonoBehaviour {
    [SerializeField] private SpriteRenderer explosionRadiusIndicator;
    private float timer = 0f;                                           
    private const float BLINKING_SPEED = .5f;
    private const float BLINK_ALPHA = .4f;
    private Color color;
    private Animator animator;
    private PlayerBehaviour playerScript;
    private const float TRIGGER_RADIUS = 20f;
    private const float EXPLOSION_TIMER = 6f;
    private const float EXPLOSION_TRIGGER_TIMER = 1.5f;
    private const float EXPLOSION_RADIUS = 4f;
    private bool isExploding = false;
    private float explosionTimer = 0f;
    private bool exploded = false;
    private const float BLINKING_SPEED_TRIGGERED = .1f;
    private const float DAMAGE_DONE = 50f;
    private bool movingToLocation;
    Vector3 targetPos;
    private float distanceToMove;
    private float movingTimer;
    float movingTime = .5f;
    private bool waitingToExplode;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start() {

    }
    private void Awake() {
        explosionRadiusIndicator = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

    }
    public void MoveToLocation(Vector3 target) {
        movingToLocation = true;
        targetPos = target;
        distanceToMove = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.y - transform.position.y, 2));
       
        startPos = transform.position;
        movingTimer = 0f;
        //BlinkIndicator();
    }

    // Update is called once per frame
    void Update() {
        if (movingToLocation) {

            if (movingTimer < movingTime) {
                transform.position = Vector3.Lerp(startPos, targetPos, movingTimer / movingTime);
                movingTimer += Time.deltaTime;
            }
            else {
                explosionRadiusIndicator.transform.gameObject.SetActive(true);
                movingToLocation = false;
            }
        }
        else if (!waitingToExplode) {
            if (!exploded) {

                if (isExploding) {
                    //actually explode if timer is up
                    if (explosionTimer >= EXPLOSION_TRIGGER_TIMER) {
                        exploded = true;
                        animator.SetTrigger("Explode");
                        explosionRadiusIndicator.gameObject.SetActive(false);
                    }
                    //otherwise incriment timer and blink indicator
                    else {
                        explosionTimer += Time.deltaTime;
                        if (timer > BLINKING_SPEED_TRIGGERED) {
                            BlinkIndicator();
                        }
                        else {
                            timer += Time.deltaTime;
                        }
                    }
                }
                else {
                    CheckStartExplosion();
                    if (explosionTimer >= EXPLOSION_TIMER) {
                        StartExplosionRoutine();
                    }
                    else {
                        explosionTimer += Time.deltaTime;
                        
                    }

                }
            }
        }
    }
    //set flag to start explosion count down
    public void StartExplosionCountdown() {
        waitingToExplode = false;
    }
    
    //swap alpha between 0 and Blink alpha constant of the explosion indicator
    void BlinkIndicator() {
        color = explosionRadiusIndicator.color;
        color.a = color.a > 0 ? 0 : BLINK_ALPHA;
        explosionRadiusIndicator.color = color;
        timer = 0f;
    }
    //start the exploding routine which increases blink speed and will explode after a short timer
    private void StartExplosionRoutine() {
        isExploding = true;
        explosionTimer = 0f;
        timer = 0f;
    }
    public void Remove() {
        Destroy(gameObject);
    }
    //do damage to player if in range
    public void DoDamage() {
        if (playerScript.CheckCollision(gameObject, EXPLOSION_RADIUS, 0, 0)) {
            playerScript.TakeDamage(DAMAGE_DONE, false);
        }
    }
    //simple check to see if player center is in range of explosion, does not take into account player hitbox just location
    public void CheckStartExplosion() {
        float magnitudeSquared = Mathf.Pow(transform.position.x - playerScript.transform.position.x, 2) + Mathf.Pow(transform.position.y - playerScript.transform.position.y, 2);
        if (magnitudeSquared <= TRIGGER_RADIUS) {
            StartExplosionRoutine();
        }
    }
}

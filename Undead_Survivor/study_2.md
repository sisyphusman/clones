# 골드메탈 뱀서라이크 6 ~ 10

## 06

### 프리펩 만들기

- Prefebs 폴더 만든 후 Enemy를 폴더로 드래그
  
- 이후 Enemy A, B를 재사용 가능

- Scale의 체인을 활성화 하면 동일한 비율로 조절

- 유니티는 생성 Instantiate와 삭제 Destroy 함수를 제공 (자주 사용시 메모리 문제)

- empty -> PoolManager 생성, PoolManager 스크립트 생성 후 부착

- Prefebs 안에 있는 몬스터들을 PoolManager의 Prefebs 이름에 드래그

- PoolManager.cs

```c# 
  // 프리펩들을 보관할 변수
  public GameObject[] prefabs;

  // 풀 담당을 하는 리스트들
  List<GameObject>[] pools;

  void Awake()
  {
      pools = new List<GameObject>[prefabs.Length];
      
      for (int index = 0; index < pools.Length; index++)
      {
          pools[index] = new List<GameObject>();
      }
  }
```

- Awake()에서 메모리 할당

&nbsp;

### PoolManager 생성

- Player 밑에 Empty Create -> Spawner 이름 변경, Spawner 스크립트 생성

- GameManager.cs
```c#
 public PoolManager pool;
```
- GameManager의 Pool에 PoolManager 오브젝트 등록

- Spawner.cs
```c#
void Update()
{
    if (Input.GetButtonDown("Jump"))
    {
        GameManager.instance.pool.Get(1);
    }
}
```
  
- Enemy.cs
```c#
private void OnEnable()
{
    target = GameManager.instance.player.GetComponent<Rigidbody2D>();
}
```

- 프리펩은 장면의 오브젝트에 접근 불가, 그래서 몬스터의 스크립에서
  플레이어의 위치를 가져와야 함
 
&nbsp;

### 주변에 생성하기

- Transform가 하는 일
  - 공간 정보: position(위치), rotation(회전), localScale(크기)
  - 계층 링크: parent(부모 Transform), childCount/GetChild(i)(자식들)

- Spawner에 Empty 생성 후 이름을 Point

- Point를 메인 화면 밖으로 많이 생성

- Spawner.cs
```c#
public Transform[] spawnPoint;

float timer;
private void Awake()
{
    spawnPoint = GetComponentsInChildren<Transform>();
}
private void Update()
{
    timer += Time.deltaTime;

    if (timer > 0.2f)
    {
        Spawn();
        timer = 0;
    }
}
private void Spawn()
{
   GameObject enemy = GameManager.instance.pool.Get(Random.Range(0, 2));
   enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;          // 0은 spawnPoint이므로 제외
}

```

- 본인 + 모든 자식의 Transform을 배열로 만듬

- 배열의 처음은 항상 자기 자신의 transform

- 매 프레임 경과시간을 누적하다가 0.2초가 넘으면 Spawn() 호출 후 타이머 초기화 → 초당 약 5마리 스폰

- 생성한 적들을 스포너 오브젝트의 자식으로 붙여서, 정리·이동·비활성화를 한 번에 관리

## 06+

- Spawner.cs
```c#
public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoint;

    int level;
    float timer;
    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 10f);          // 소수점 내림

        if (timer > (level == 0 ? 0.5f : 0.2f))
        {
            Spawn();
            timer = 0;
        }
    }
    private void Spawn()
    {
       GameObject enemy = GameManager.instance.pool.Get(level);
       enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;          // 0은 spawnPoint이므로 제외
    }
}
```

- 시간에 따른 레벨 구간 생성

- 레벨에 따라서 스폰의 몹이 달라짐


### 소환 데이터

```c#
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
```

- 만든 클래스를 그대로 타입으로 활용하여 배열 변수 선언

- Inspector에서는 보이기 위해서는 직렬화가 필요

- 클래스 위에 [System.Serializable] 라는 속성을 입력
  
- Spawner의 Inspector에서 Spawn Data 수정
  - Type, Time, Health, Speed
  
- Prefabs에서 몬스터 B 삭제 후 몬스터 A를 몬스터로 수정

### 소환 적용하기

- PoolManager에서 Prefabs 하나 지우기

- Enemy.cs
```c#
...

public float speed;
public float health;
public float maxHealth;
public RuntimeAnimatorController[] animCon;
Animator anim;

...

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    ...

        private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }
```

- anim의 runtimeAnimatorController으로 type을 정함

- 나머지 데이터를 Unity Inspector 값으로 초기화

- 몬스터가 재사용되는 이유는, 오브젝트 풀링을 쓰고 있기 때문
PoolManager.Get()이 비활성화된 기존 적 오브젝트를 다시 활성화해서 넘겨주니까, 새로 Instantiate하지 않고 같은 인스턴스를 반복해서 사용


- 흐름을 한 줄로
```
Spawner.Spawn() → pool.Get(0) 호출

PoolManager.Get()이 pools[0] 리스트를 훑어서 !item.activeSelf (비활성)인 놈을 찾음 → 찾으면 item.SetActive(true) 하고 반환

이때 활성화되면서 Enemy.OnEnable() 이 즉시 실행 → isLive = true; health = maxHealth; 등 상태 초기화

이어서 Spawner가 enemy.GetComponent<Enemy>().Init(spawnData[level]) 로 속도/체력/애니메이터 세팅

나중에 적이 죽거나 화면 밖 처리 시 gameObject.SetActive(false)로 꺼 두면, 다음 스폰 때 다시 그 객체가 재사용됨
```

## 07

### 무기 만들기

- 스프라이트에서 Bullet 0을 가져와서 Scene에 드래그 후 Bullet Script 장착

- bullet.cs  
```c#
public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    public void Init(float damege, int per)
    {
        this.damage = damege;
        this.per = per;
    }
}
```

- Enemy.cs
```c#
private void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.CompareTag("Bullet"))
        return;

    health -= collision.GetComponent<Bullet>().damage;

    if (health > 0)
    {
        // Live, Hit Action
    }
    else
    {
        Dead();
    }
}

void Dead()
{
    gameObject.SetActive(false);
}
```

- 충돌이 일어나면 health의 값을 데미지만큼 줄어듬

- health가 0보다 작으면 Dead() -> SetActive(false)로 삭제된 것처럼 사용

- Bullet 스크립트의 데미지를 11로 조정

&nbsp;

- Player 밑에 Empty로 Weapon 0을 생성 후 Weapon Script 부착

- Weapon.cs
```c#
 class Weapon : MonoBehaviour
{
    public int id;
    public int prefabid;
    public float damage;
    public int count;
    public float speed;
    private void Start()
    {
        Init();        
    }

    private void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                break;
        }
    }
    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            default:
                break;
        }
    }
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet = GameManager.instance.pool.Get(prefabid).transform;
            bullet.parent = transform;
            bullet.GetComponent<Bullet>().Init(damage, -1);         // per은 무한 관통
        }
    }
}
```

- Weapon의 Inspector에서 Prefabid = 1, Damage = 11, Count = 1 설정
  
&nbsp;

```c#
void Batch()
{
    for (int index = 0; index < count; index++)
    {
        Transform bullet = GameManager.instance.pool.Get(prefabid).transform;
        bullet.parent = transform;

        Vector3 rotVec = Vector3.forward * 360 * index / count;
        bullet.Rotate(rotVec);
        bullet.Translate(bullet.up * 1.5f, Space.World);
        bullet.GetComponent<Bullet>().Init(damage, -1);         // per은 무한 관통
    }
}
```

- 무기를 Count에 맞게 자동 생성 후 360 * index / count 만큼 각도 지정

### 레벨에 따른 배치

- Weapon.cs
```c#
private void Update()
{
    switch (id)
    {
        case 0:
            transform.Rotate(Vector3.back * speed * Time.deltaTime);
            break;
        default:
            break;
    }
    
    // 테스트 코드
    if (Input.GetButtonDown("Jump"))
    {
        LevelUp(20, 5);
    }
}
public void LevelUp(float damage, int count)
{
    this.damage = damage;
    this.count += count;

    if (id == 0)
        Batch();
    
}

public void Init()
{
    switch (id)
    {
        case 0:
            speed = 150;
            Batch();
            break;
        default:
            break;
    }
}
private void Batch()
{
    for (int index = 0; index < count; index++)
    {
        Transform bullet;

        if (index < transform.childCount)
        {
            bullet = transform.GetChild(index);
        }
        else
        {
            bullet = GameManager.instance.pool.Get(prefabid).transform;
            bullet.parent = transform;
        }

        bullet.localPosition = Vector3.zero;
        bullet.localRotation = Quaternion.identity;

        Vector3 rotVec = Vector3.forward * 360 * index / count;
        bullet.Rotate(rotVec);
        bullet.Translate(bullet.up * 1.5f, Space.World);
        bullet.GetComponent<Bullet>().Init(damage, -1);         // per은 무한 관통
    }
}
```

```c#
if (index < transform.childCount)
{
    bullet = transform.GetChild(index);
}
else
{
    bullet = GameManager.instance.pool.Get(prefabid).transform;
    bullet.parent = transform;
}
```

- 기존 오브젝트를 먼저 활용하고 모자란 것은 풀링에서 가져오기

&nbsp;

```c#

bullet.localPosition = Vector3.zero;
bullet.localRotation = Quaternion.identity;
```

- 재사용되는 총알이나 이미 자식으로 붙어있는 오브젝트가 이전 사용에서 남긴 위치/회전값을 제거한 후 사용

## 08

### 몬스터 검색 구현

- 레이어: 물리, 시스템 상으로 구분짓기 위한 요소

- prefabs에서 enemy 누른 후 Layer 추가, 8번째에 Enemy 생성, 선택

- 범위, 레이어, 스캔 결과 배열, 가장 가까운 목표를 담을 변수 선언

- Scanner.cs

```c#
public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    private void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }
    
    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }
}
```

- 현재 원에서 가장 가까운 몬스터를 타켓으로 검색

### 프리펩 만들기

- Bullet prefabs를 Hierachy에 드래그, 원하는 스프라이트를 Bullet 스프라이트에 드래그

- Box Collider 2D에서 Reset하여 박스 크기 맞춤

- 총알의 컴포넌트에서 Capture Collider 2d를 추가, 이후 Box 콜라이더 삭제

- 새로 만든 Bullet을 Prefabs에 드래그, Original Prefabs 선택

- Bullet의 IsTrigger 체크

### 총탄 생성하기

```c#
...
Player player;

private void Awake()
{
    player = GetComponentInParent<Player>();
}
...

private void Update()
{
    switch (id)
    {
    case 0:
        transform.Rotate(Vector3.back * speed * Time.deltaTime);
        break;
    default:
        timer += Time.deltaTime;

        if (timer > speed)
        {
            timer = 0f;
            Fire();
        }

        break;
    }
}
...

public void Init()
{
    switch (id)
    {
        case 0:
            speed = 150;
            Batch();
            break;
        default:
            speed = 0.3f;
            break;
    }
}

...

void Fire()
{
    if (!player.scanner.nearestTarget)
        return;

    Transform bullet = GameManager.instance.pool.Get(prefabid).transform;
    bullet.position = transform.position;
}
```

- player = GetComponentInParent<Player>(); 
  - 내 자신 또는 위쪽 부모 오브젝트들 중에서 Player 컴포넌트를 찾아서 변수에 캐싱

- Fire()에서 스캐너 검사 이후 몹이 있으면 그 자리에 총알 생성

- Init()의 default의 speed는 연사 속도 

### 총탄 발사하기

- 총탄은 속도가 필요하므로 RigidBody2D 추가, 중력은 0으로 설정

- Bullet.cs
```c#
public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per > -1)               // 관통이 -1 (무한)보다 큰 것에 대해서는 속도 적용
        {
            rigid.velocity = dir * 15f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--;

        if (per == -1)
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
```

- Bullet.cs에서 RigidBody2D 추가

- Init에 방향 추가

- 관통이 -1보다 큰 것에 속도 적용

- 적이 아니거나 근접 무기일때는 충돌 처리 안함

- 관통 값이 줄어들면서 -1 이면 총알 비활성화, 속도 초기화


&nbsp;

- Weapon.cs

```c#
...
void Fire()
{
    if (!player.scanner.nearestTarget)
        return;

    // 총알 나가는 방향 설정
    Vector3 targetPos = player.scanner.nearestTarget.position;
    Vector3 dir = targetPos - transform.position;
    dir = dir.normalized;

    // 회전 설정
    Transform bullet = GameManager.instance.pool.Get(prefabid).transform;
    bullet.position = transform.position;
    bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
    bullet.GetComponent<Bullet>().Init(damage, count, dir);         // per은 무한 
}
...

// Bullet.cs
...

public void Init(float damage, int per, Vector3 dir)
{
    this.damage = damage;
    this.per = per;

    if (per > -1)               // 관통이 -1 (무한)보다 큰 것에 대해서는 속도 적용
    {
        rigid.velocity = dir * 15f;
    }
}
...
```

- 총알의 관통이 남아 있다면 Init할때 15로 속력 설정

## 09 

### 몬스터 처치

- 코루틴 Coroutine: 생명 주기와 비동기처럼 실행되는 함수  
```c#
IEnumerator KnockBack()
{
    yield return null; // 1 프레임 쉬기
    yield return new WaitForSeconds(2f); // 2초 쉬기
}
```

- Enemy.cs
```c#
...

WaitForFixedUpdate wait;

...

private void Awake()
{
    rigid = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    spriter = GetComponent<SpriteRenderer>();
    wait = new WaitForFixedUpdate();
}

...

// Base Layer 1개이므로 0번, Hit이면 건너뜀
if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) 
    return;

private void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.CompareTag("Bullet"))
        return;

    health -= collision.GetComponent<Bullet>().damage;
    StartCoroutine(KnockBack());

    if (health > 0)
    {
        anim.SetTrigger("Hit");         // AC.Transition.Hit Condition 체크
    }
    else
    {
        Dead();
    }
}

IEnumerator KnockBack()
{
    yield return wait; // 하나의 물리 프레임 딜레이
    Vector3 playerPos = GameManager.instance.player.transform.position;
    Vector3 dirVec = transform.position - playerPos;
    rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
}

```

&nbsp;

### 사망 리액션

- Enemy.cs
```c#
Collider2D coll;

...

coll = GetComponent<Collider2D>();

...

private void OnEnable()
{
    target = GameManager.instance.player.GetComponent<Rigidbody2D>();
    isLive = true;
    coll.enabled = true;
    rigid.simulated = true;
    spriter.sortingOrder = 2;
    anim.SetBool("Dead", false);
    health = maxHealth;
}

...

private void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.CompareTag("Bullet"))
        return;

    health -= collision.GetComponent<Bullet>().damage;
    StartCoroutine(KnockBack());

    if (health > 0)
    {
        anim.SetTrigger("Hit");         // AC.Transition.Hit Condition 체크
    }
    else
    {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);
    }
}
```

- 충돌 시 hp가 0 이하일때 isLive, coll, rigid를 false로 설정

- 스프라이트 레이어를 1로 설정

- anim의 Dead 플래그 킴

- 생성 시 위 옵션을 반대 설정

- Dead Enemy 1에서 0초때 애니메이션을 1초대에 복사

- 1초대에서 Add Event 생성 후 Dead() 함수 등록

### 처치 데이터 얻기

- GameManager.cs
```c#
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("# Player Info")]
    public int level;
    public int kill;
    public int exp;

    // 각 레벨의 필요경험치
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;

    private void Awake()
    {
        instance = this;   
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }

    public void GetExp()
    {
        exp++;

        if (exp == nextExp[level])
        {
            level++;
            exp = 0;
        }
    }
}
```

- 게임 데이터들을 게임 매니저에서 관리

- 필요 경험치를 배열로 하드코딩

- 경험치 얻는 코드 추가

- Inspector에서 보기 쉽게 Header 사용, Next Exp 배열 안보이면 스크립트 Reset

&nbsp;

- Enemy.cs
```c#
private void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.CompareTag("Bullet") || !isLive)
        return;

    health -= collision.GetComponent<Bullet>().damage;
    StartCoroutine(KnockBack());

    if (health > 0)
    {
        // AC.Transition.Hit Condition 체크
        anim.SetTrigger("Hit");         
    }
    else
    {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);

        GameManager.instance.kill++;
        GameManager.instance.GetExp();
    }
}
```

- 자신이 죽었을때 게임 매니저에서 kill 증가와 경험치 획득


## 10

### HUD 제작하기

- Hierarchy - UI - Canvas 생성

- Canvas - UI - Legacy - Text 생성

- 텍스트 설정(폰트 크기, 폰트체, width, height, 정렬)

- Canvas에서 UI Scale Mode - Scale With Screen Size로 설정 (해상도 관계없이)

- Ref Res: x = 135, y = 270

- Match = 0.5

- Ref PPU: 18

### HUD 스크립트

- HUD.cs
```c#
public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:

                break;
            case InfoType.Level:

                break;
            case InfoType.Kill:

                break;
            case InfoType.Time:

                break;
            case InfoType.Health:

                break;
        }
    }
}
```

- 기본적인 틀 구현

### 경험치 게이지

- Canvas - UI - Slider 생성

- 앵커: UI 오브젝트의 기준점 설정, 변경 시 오른쪽 속성이 달라짐

- Height 7 제외 나머지 다 0

- Interactable 언체크

- Transition, Navigation - None

- Handle Rect - None

- Handle Slide Area - 삭제

- Background 앵커 - shift + alt 누른 채로 맨 오른쪽 아래 앵커 눌러서 꽉 채움

- Fill Area, Fill - Left, Right 0으로 초기화

- UI 나머지는 영상 골드메탈 - HUD

### 레벨 및 킬수 텍스트

- Canvas - UI - Legacy - Text

- 앵커 alt + shift 맨 오른쪽 위

- Text에 준비한 폰트, 사이즈 조절

- 이미지 UI도 똑같이 생성 

- Set Native Size: 오브젝트 크기를 스프라이트의 원래 크기로 변경

- 위치 조절

- Exp 텍스트, Level 텍스트, Kill 이미지, Kill 텍스트 추가

### 타이머 텍스트

- 앵커로 위치 잡고 포지션 0, -8

- HUD.cs
```c#
private void LateUpdate()
{
    switch (type)
    {
        case InfoType.Exp:
            float curExp = GameManager.instance.exp;
            float maxExp = GameManager.instance.nextExp[GameManager.instance.level];
            mySlider.value = curExp / maxExp;
            break;
        case InfoType.Level:
            myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
            break;
        case InfoType.Kill:
            myText.text = string.Format("{0:F0}", GameManager.instance.kill);
            break;
        case InfoType.Time:
            float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
            int min = Mathf.FloorToInt(remainTime / 60);
            int sec = Mathf.FloorToInt(remainTime % 60);
            myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
            break;
        case InfoType.Health:

            break;
    }
}
```

- 게임 매니저에 있는 데이터를 string으로 포맷에 맞게 변환해서 입력

### 체력 게이지

- Create Empty로 Health로 생성

- Slider를 복제해서 Health Slider를 생성

- Background와 Fill의 그림을 변경

- HUD.cs
```c#
case InfoType.Health:
    float curHealth = GameManager.instance.health;
    float maxHealth = GameManager.instance.maxHealth;
    mySlider.value = curHealth / maxHealth;
    break;
```

- Health 타입일때 생성

- GameManager에서 void Start() { health = maxHealth; } 추가함

- Follow.cs
```c#

public class Follow : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
```

- 헬스바가 캐릭터를 정확하게 따라가게 만듬
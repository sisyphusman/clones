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


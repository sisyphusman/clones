# 골드메탈 뱀서라이크 1 ~ 5

## 01

- 유니티 2021.3.13.f1, Android Build Support, 2D URP

- 에셋 패키지 이용

- 스프라이트 설정
    - Pixel Per Unit: 상대 크기 조절
    - Filter Mode: Point
    - Compression: None
    - Sprit Mode: Multiple
  
- Sprite Editor
  - 픽셀 사이즈: 18, 20
  - 패딩: 1, 1
  - 알파값만 확인 가능
  - Slice
  - 각 스프라이트마다 이름 변경 가능

- Sprite 하나를 Scene에 드래그
  
- Scene 3D 아이콘 크기는 기즈모 버튼에서 조절

- Component 설정  
  - Rigidbody 2D 적용
  - Capsule Colider 2D X(0.6), Y(0.9)

- Props
  - Shadow를 Hierarchy창의 Player에 드래그
    - Player는 레이어 5
    - Shadow는 레이어 0

- 게임 배경색은 Camera의 Inspector의 Environment의 Background에서 변경

## 02

- 폴더 생성 - Scripts
  
- Player는 MonoBehaviour 클래스를 상속

- ```c#
  ...
  public Vector2 inputVec;

  void Update()
  {
    inputVec.x = Input.GetAxis("Horizontal");
    inputVec.y = Input.GetAxis("Vertical");
  }
  ```

- Edit - Project Setting - Input Manager - Axes - Horizontal, Vertical

- class는 Player, Start(), Update() 임을 주의

- Axis 값은 -1 ~ 1

- 이동의 3가지 방법
  ```c#
  // 1. 힘을 준다
  rigid.AddForce(inputVec);

  // 2. 속도 제어
  rigid.velocity = inputVec;

  // 3. 위치 이동
  rigid.MovePosition(rigid.position + inputVec);
  ```

- 대각선 길이 비율 때문에 노멀라이즈, CPU의 속도를 똑같이 하기 위하여 델타 타임 사용
  
  ```c#
  Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
  ```

- 부드러운 이동 보정 없는 함수

  ```c#
  inputVec.x = Input.GetAxisRaw("Horizontal");
  inputVec.y = Input.GetAxisRaw("Vertical");
  ```

## 02+

- Input Manager가 아니라 Input System을 사용해서 구현
  
- Window - Package Manager - Unity Registry - Input System 설치

- Player Component에 Player Input 추가

- Create Actions 추가, Player 프로필 에셋 저장

- Input Actions에서 Processors - Normalize Vector 2 추가 후 저장 혹은 Auto-Save

- 함수 안나올때 유니티에서 Edit > Preferences > External Tool > Regenerate Project 

- Input System 적용시

```c#
using UnityEngine.InputSystem;

void FixedUpdate()
{
    Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
    rigid.MovePosition(rigid.position + nextVec);
}

void OnMove(InputValue value)
{
    inputVec = value.Get<Vector2>();
}
```

## 03

- SpriteRenderer는 이미지를 반전시키는 Flip 속성이 있음

- 
```c#

SpriteRenderer spriter;

void Awake()
{
    spriter = GetComponent<SpriteRenderer>();
}

void LateUpdate()
{
    if (inputVec.x != 0)
    {
        // 왼쪽으로 갈때만 체크
        spriter.flipX = inputVec.x < 0;
    }
}
```

#### 셀 애니메이션

- Sprites에서 애니메이션을 만들 스프라이트들을 모아서 Player에 드래그, 파일 저장

- Animator 더블 클릭 시 Animator 창이 나옴

- Set as Layer Default State (기본 상태 변경)

- Make Transition (이동경로 생성)

- Animator - Parameters에서 Conditions 생성 가능

- Player Component에 Animator 적용

&nbsp;

- Transition 클릭 후 Conditions에서 조건 추가 가능

- Transition - Settings - Transition Duration -> 0.01로 최소화 (부드럽게 전환해주는 역할을 최소화)

- Has Exit Time 체크 해제 - 즉각적인 반응

&nbsp;

- Animation에서 Loop Time 체크 해제

- 벡터에서 크기 반환

```c#
    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);
    ...
    }
```

- Save & Save Project

- 다른 캐릭터의 애니메이션들도 똑같이 Player에 드래그 한 후 숫자만 바꾼 후, 이전 애니메이터에 생긴 것들은 삭제

- 기존 애니메이터 클릭 후 Project 창 밑에 + 누름 Animator Override Controller를 누름 -> 새로운 애니메이터에 컨트롤러 지정 후 덮어쓸 것들을 지정

- Player ctrl + D로 복제 -> 새로운 애니메이터 지정

## 04

- 무한 타일맵 만들기

- Window - 2D - Tile Palette -> 타일 팔렛트 뷰 열기

- Project 밑 + -> 2D -> Tiles -> Rule Tile 생성 -> Number of Tiling Rules = 1 설정

- 그리고 Output은 Random 설정 -> Size도 10으로 변경

- 마지막으로 Inspector 자물쇠 잠금

- Sprites에서 타일을 타일맵의 Size 밑에 드래그, 끝난 후 잠금 해제

- 완성된 타일 에셋은 팔레트 창에서 드래그 드랍

- Hierarchy 밑 + -> 2D Object -> Tilemap -> Rect

- 타일 팔레트에서 라인 브러쉬 선택 후 밑으로 10x10, 위로 10x10 그림

- Tilemap에 Tilemap Collider 2D 추가
  - Used By Composite 체크

- Tilemap에 Composite Collider 2D 추가
  - Is Trigger 체크

- Inspecter 창에서 Add Tag -> Ground 추가

- Tilemap은 Ground 설정

- Player에서 Empty 추가, 이름은 Area
  - Box Collider 2D 추가 이후 Size는 20, 20 설정
  - Is Trigger = Player 충돌 X
  - Tag는 Area 설정

- Tilemap에 Reposition 스크립트 적용

- GameManager Empty로 생성 이후 GameManager 스크립트 적용

- GameManager.cs
```c#
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    void Awake()
    {
        instance = this;   
    }
}
```
- 정적 변수는 Inspector에 안나옴
- 정적 변수는 즉시 다른 클래스에서 부를 수 있음

&nbsp;

- Ground에 적용할 Script
- 플레이어가 ground의 좌표에서 멀어지면 ground가 해당 방향으로 (20*2)만큼 이동
  
```c#
public class Reposition : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;

        // ground의 좌표
        Vector3 myPos = transform.position;                       
        
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy":

                break;
        }
    }
}

```

- Scene에 5번째 스냅핑을 Move 10 으로 설정

- Tilemap 누르고 Move Tool 모드에서 화살표 누른 상태에서 Ctrl

#### 카메라

- 카메라에 Pixel Perfect Camera 컴포넌트 추가

- PPU를 18로 설정

- X를 135, Y를 270으로 설정

- Game 창에서 Simulator로 바꿀 수 있음

- Package Manager에서 Cinemachine 설치

- Hierarchy에서 추가 -> Cinemachine -> Virtual Camera 생성

- Virtual Camera에서 Follow을 Player로 지정

- Main Camera -> Update Method에서 Fixed Update로 설정

- Tilemap의 레이어 -1로 지정

## 05

- 몬스터 Sprite를 펼치고 Run 0를 하이어라키로 드래그 드랍

- 몬스터 Sprite 랜더러의 레이어를 2로 변경

- 그림자를 몬스터에 추가 (x = 0, y = -0.45)

- 몬스터에 애니메이터 부착 후 미리 생성된 AcEnemy 1 적용

- 중력 크기를 0, Freeze Rotation 체크

- Player의 Mass를 15로 설정

- Enemy가 Player를 따라가는 스크립트 부착

- 몬스터에서 플레이어의 방향을 구해서 speed만큼 이동하고 속도를 0으로 고정

```c#
public class Enemy : MonoBehaviour
{
    public float speed;
    public Rigidbody2D target;

    bool isLive = true;

    Rigidbody2D rigid;
    SpriteRenderer spriter;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!isLive)
          return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        spriter.flipX = target.position.x < rigid.position.x;           // 몬스터가 타켓보다 더 오른쪽에 있으면 플립
    }
}
```

&nbsp;

- Enemy에 Reposition 스크립트 추가
```c#
...
  Collider2D coll;

  void Awake()
  {
      coll = GetComponent<Collider2D>();
  }

  case "Enemy":
    if (coll.enabled)
    {
        transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));                // 몬스터가 반대 방향에서 나타남
    }
    break;
...
```
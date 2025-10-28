# 골드메탈 뱀서라이크 11 ~ 15

## 11

### 아이템 데이터 만들기

- ItemData.cs

```c#
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# Weapon")]
    public GameObject projectile;
}

```

- Scriptable Object: 다양한 데이터를 저장하는 에셋
- Data 폴더 생성 후 ScriptableObject의 itemData 생성, item 0으로 이름 변경 후 item name은 삽
- Item Icon: 스프라이트 - UI - Select 0
- Weapon의 Projectile을 프리펩의 Bullet 0으로 지정

### 레벨업 버튼 제작

- Create Empty 생성 후 Level Up이라고 지정, 앵커는 shift + alt, 오른쪽 아래
- UI - Legacy - Button 생성 후 Level Up 아래에 위치, 이름은 Item 0
- 배경은 Panel로 지정
- UI - Legacy - Text 생성 후 Level Up 아래에 지정
- Item.cs

```c#
public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;

    Image icon;
    Text textLevel;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
    }

    private void LateUpdate()
    {
        textLevel.text = "Lv." + (level + 1);
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                break;
            case ItemData.ItemType.Glove:
                break;
            case ItemData.ItemType.Shoe:
                break;
            case ItemData.ItemType.Heal:
                break;
        }

        level++;

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
```

- GetComponentsInChildren에서 두번째 값으로 가져오기 (첫번째는 자기자신)
- Item 0에서 Item 스크립트 추가 후 4개 복제
- LevelUp의 컴포넌트에서 Vertical Layout Group 추가
- Data폴더의 Item 0등을 각 Item의 Data에 드래그
- 버튼 클릭 이벤트와 연결할 함수 추가
- public void OnClick() - 버튼 클릭 이벤트 분기
- 스크립트블 오브젝트에 작성한 레벨 데이터 개수를 넘기지 않게 로직 추가

### 무기 업그레이드

```c#
 case ItemData.ItemType.Range:
     if (level == 0)
     {
         GameObject newWeapon = new GameObject();
         weapon = newWeapon.AddComponent<Weapon>();
         weapon.Init(data);
     }
     else
     {
         float nextDamage = data.baseDamage;
         int nextCount = 0;

         nextDamage += data.baseDamage * data.damages[level];
         nextCount += data.counts[level];

         weapon.LevelUp(nextDamage, nextCount);
     }
     break;
```

- 플레이어가 가진 Weapon 오브젝트들은 삭제
- AddComponent:  게임 오브젝트에 T 컴포넌트를 추가하는 함수
- Weapon 초기화 함수 init 작성
- 각종 무기 속성 변수들을 스크립트블 오브젝트 데이터로 초기화
- 프리펩 아이디는 풀링 매니저의 변수에서 찾아서 초기화
- 스크립트블 오브젝트의 독립성을 위해서 인덱스가 아닌 프리펩으로 설정
- Awake 함수에서의 플레이어 초기화는 게임매니저 활용으로 변경
- 처음 이후의 레벌업은 데미지와 횟수를 계산
- Weapon의 작성된 레벨업 함수를 그대로 적용

### 장비 업그레이드

- 장비의 타입과 수치를 저장할 변수 선언
- Weapon과 동일하게 초기화, 레벨업 함수 작성
- 장갑의 기능인 연사력을 올리는 함수 작성
- 플레이어로 올라가서 모든 Weapon을 가져오기
- foreach문으로 하나씩 순회하면서 타입에 따라 속도 올리기
- 신발의 기능인 이동 속도를 올리는 함수 작성
- 타입에 따라 적절하게 로직을 적용시켜주는 함수 추가
- 장비가 새롭게 추가되거나 레벨업 할 때 로직적용 함수를 호출
- 버튼 스크립트에서 새롭게 작성한 장비 타입의 변수 선언
- 무기 로직과 마찬가지로 최초 레벨업은 게임오브젝트 생성 로직을 작성
- BroadcastMessage: 특정 함수 호출을 모든 자식에게 방송하는 함수

## 11+

### 양손 배치

- 왼손 오브젝트 추가
- 레이어를 플레이어보다 높게 설정
- 왼손은 Z축 회전으로 표현
- 오른손 오브젝트 추가
- 오른손은 X축 위치 이동으로 표현

### 반전 컨트롤 구현

- 손을 컨트롤하는 스크립트 생성
- 오른손, 왼손 구분을 하는 bool 변수 선언
- 플레이어의 스프라이트렌더러 변수 선언 및 초기화
- 오른손의 각 위치를 Vector3 형태로 저장
- 왼손의 각 회전을 Quatanion 형태로 저장
- 왼손, 오른손 오브젝트에 스크립트 추가하고 변수 초기화
- 플레이어의 반전 상태를 지역변수로 저장
- 왼손 회전에는 localRotation 사용
- 왼손, 오른손의 sortingOrder를 바꿔주기

### 데이터 추가

- 손 오브젝트에 연결되어 있는 스프라이트 삭제
- 손 오브젝트 비활성화
- 스크립트블 오브젝트 코드에서 손 스프라이트를 담을 속성 추가

### 데이터 연동

- 플레이어에서 손 스크립트를 담을 배열변수 선언 및 초기화
- GetComponetsInChildren<>()에서 true를 넣으면 비활성화 된 오브젝트 활성화
- Weapon 스크립트의 초기화 함수에서 로직 작성
- enum 열거형 데이터는 정수 형태로도 사용 가능
- enum 값 앞에 int 타입을 작성하여 강제 형변환

## 12

### UI 완성하기

- 바탕 이미지 오브젝트 추가 및 색상 변경
- 자식 오브젝트로 창이 될 이미지 오브젝트 추가
- 창의 제목이 될 텍스트 추가
- 기존의 능력 업그레이드를 창 오브젝트 자식으로 넣기
- Control Child Size: 자식 오브젝트를 자신의 크기에 맞게 자동 변경
- 아이템 버튼의 이미지와 텍스트는 왼쪽 기준으로 변경 및 위치 조정
- 텍스트를 복사하여 아이템 이름과 설명으로 배치
- 텍스트의 색상을 조금씩 바꾸어 구분하기
- 작업하지 않은 버튼을 지우고 새롭게 복사
- 나머지 2개 비활성화하여 사이즈 맞추기

### 아이템 텍스트

- 인스펙터에 텍스트를 여러 줄 넣을 수 있게 TextArea 속성 부여
- 설명에 데이터가 들어가는 자리는 {index} 형태로 작성
- 아이템 스크립트에 이름과 설명 텍스트 변수 추가 및 초기화
- GetComponents의 순서는 계층구조의 순서를 따라감
- 레벨 텍스트 로직은 OnEnable로 이동
- 아이템 타입에 따라 switch case문으로 로직 분리

### 창 컨트롤

- 레벨업 창을 관리하는 스크립트 생성
- RectTransform 변수 선언 및 초기화
- 보이고 숨기는 함수 작성
- 게임매니저에 레벨업 변수 선언 및 초기화
- 게임매니저의 레벨업 로직에 창을 보여주는 함수 호출
- 모든 아이템 버튼의 OnClick에 창을 숨기는 함수 연결
- 레벨업 오브젝트의 크기를 미리 0으로 설정

### 기본무기 지급

- 레벨업 스크립트에서 아이템 배열 변수 선언 및 초기화
- 버튼을 대신 눌러주는 함수 작성

### 시간 컨트롤

- 시간 정지 여부를 알려주는 bool 변수 선언
- 시간 정지, 작동하는 함수 두 개 작성
- timeScale: 유니티의 시간 속도 (배율)
- 레벨 업 창이 나타나거나 사라지는 타이밍에 시간 제어
- 각 스크립트의 Update 계열 로직에 조건 추가하기

### 랜덤 아이템

- 아이템 스크립트에 랜덤 활성화 함수 작성
- foreach를 활용하여 모든 아이템 오브젝트 비활성화
- 랜덤으로 활성화 할 아이템의 인덱스 3개를 담을 배열 선언
- 3개 데이터 모두 Random.Range 함수로 임의의 수 생성
- 서로 비교하여 모두 같지 않으면 반복문을 빠져나가도록 작성
- for문을 통해 3개의 아이템 버튼 활성화
- 아이템이 최대 레벨이면 소비 아이템이 대신 활성화 되도록 작성
- Min 함수를 사용하여 최고 경험치를 그대로 사용하도록 변경
- 원활한 테스트를 위해 필요 경험치를 낮추기

## 13

### 게임 시작

- 타이틀을 담당하는 이미지 오브젝트 추가
- Shadow: UI 그래픽을 기준으로 그림자를 생성하는 컴포넌트
- 컬러 선택 창의 스포이드로 자유로운 색상 선택 기능
- 버튼의 Navigation 속성을 None으로 변경
- Outline: UI 그래픽을 기준으로 외각선을 그리는 컴포넌트
- 게임 오브젝트의 함수 SetActive는 Onclick 이벤트에 바로 사용 가능
- HUD 관련된 5가지 오브젝트를 새로운 오브젝트에 넣어두기
- HUD 오브젝트는 미리 비활성화
- 게임매니저의 기존 Start 함수를 GameStart로 변경

### 플레이어 피격

- 플레이어 스크립트에 OnCollisionEnter2D 이벤트 함수 작성
- Time.deltaTime을 활용하여 적절한 피격 데미지 계산
- 게임매니저의 생명력 관련 변수는 float로 변경
- 생명력이 0보다 작다를 if 조건으로 작성
- GetChild: 주어진 인덱스의 자식 오브젝트를 반환하는 함수
- 애니메이터 SetTrigger 함수로 죽음 애니메이션 실행

### 게임 오버

- 완성된 게임시작 오브젝트를 복사하여 게임결과로 활용
- 게임시작 버튼도 게임재시작 전용으로 편집
  - 연결 함수들 삭제
- 타이틀에 Set Native Size를 설정하여 사이즈 조절
- 게임매니저에 게임 재시작 함수 작성
- 장면 관리를 사용하기 위해 SceneManagement 네임스페이스 추가
- LoadScene: 이름 혹은 인덱스로 장면을 새롭게 부르는 함수
- 게임 오버 담당 함수 작성
- 딜레이를 위해 게임오버 코루틴도 작성
- 게임결과 UI 오브젝트를 저장할 변수 선언 및 초기화
- 게임오버 함수를 플레이어 스크립트에의 사망 부분에서 호출하도록 작성
- 게임 실행 중, 인스펙터를 수정하면 게임 종료시 원래대로 돌아옴
- 게임 재시작 버튼의 OnClick에 재시작 함수 연결

### 게임 승리

- 몬스터 정리 전용 빈 오브젝트 생성 (UI 아님)
- EnemyCleaner 오브젝트 생성 후 데미지 조절, 총알 태그와 콜라이더의 IsTrigger 설정 (종료 시 모든 몹 제거 용도)
- 게임 결과 오브젝트 내에서 게임 승리를 위한 이미지 추가
- 게임 결과를 보여주는 스크립트를 새롭게 생성 및 컴포넌트 등록
- 이미지 오브젝트를 활성화하는 승리, 패배 함수 하나씩 작성
- 게임 매니저의 인스펙터를 다시 확인하여 변수 초기화
- 게임 결과 스크립트의 인스펙터에서도 변수 초기화
- 게임 승리할 때 적을 정리하는 클리너 변수 선언 및 초기화
- 게임 승리 로직은 게임 오버 로직을 복사해서 편집
- 게임 시작 함수 내에 시간 재개 함수 호출
- 경험치 얻는 함수에도 isLive 필터 추가
- 게임 결과의 타이틀 오브젝트는 모두 비활성화

## 14

### 캐릭터 선택 UI

- GameStart에서 Character Group을 생성 후 Grid Layout Group 컴포넌트 부착
- Grid Layout Group: 자식 오브젝트를 그리드 형태로 정렬하는 컴포넌트
- 시작 버튼을 자식 오브젝트로 넣기
- 캐릭터 선택 버튼으로 편집
- 글자(이름, 효과) 설정, Outline 설정

### 선택 적용하기

- 게임매니저에서 캐릭터 ID를 저장할 변수 선언
- 기존 무기 지급을 위한 함수 호출에서 인자 값을 캐릭터ID로 변경
- 플레이어 오브젝트는 미리 비활성화
- 게임 시작할 때 플레이어 활성화 후 기본 무기 지급
- 플레이어 스크립트에 여러 애니메이터 컨트롤러를 저장할 배열 변수 선언
- Player.cs에 OnEnable 함수 추가 후, 애니메이터 변경 로직 추가
- 캐릭터 선택 버튼의 게임 시작 함수를 재연결

### 캐릭터 특성 로직

- 캐릭터 특성을 관리하는 새로운 스크립트 생성
- 함수가 아닌 속성을 작성
- 삼항연산자를 활용하여 캐릭터에 따라 특성치 적용
- 평소 많이 활용한 클래스들의 속성들도 이런 방식
- 캐릭터 특성치에 맞는 각종 속성들을 작성

## 14+


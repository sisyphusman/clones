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


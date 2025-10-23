# 유니티 단축키 & 팁

에디터 전역 & 재생

새 씬/열기/저장: Ctrl+N / Ctrl+O / Ctrl+S

다른 이름으로 저장: Ctrl+Shift+S

재생 / 일시정지 / 한 프레임 진행: Ctrl+P / Ctrl+Shift+P / Ctrl+Alt+P

Build Settings: Ctrl+Shift+B

현재 탭 최대화 토글: Shift+Space

되돌리기/다시하기: Ctrl+Z / Ctrl+Y

도구 전환(툴바)

Hand(손): Q

Move(이동): W

Rotate(회전): E

Scale(크기): R

Rect Tool(UI/2D): T

(통합 Transform 툴이 있는 버전) Y

씬 뷰 네비게이션

선택 오브젝트로 초점/프레임: F

선택 오브젝트 추적(가까이 붙기): Shift+F

카메라를 현재 뷰에 정렬: Ctrl+Shift+F
(카메라 선택 후 사용하면 “씬 뷰 = 카메라”로 딱 맞춤)

궤도/패닝/줌: Alt+좌클 드래그 / 휠 클릭 드래그 / 휠

플라이 모드: 우클릭 유지 + WASD 이동, Q/E 상하, Shift 가속
(속도는 휠로 조절 가능)

트랜스폼 & 스냅

복제: Ctrl+D

그리드 스냅 이동/회전/스케일: 드래그 중 Ctrl 유지

버텍스 스냅: V 누른 채 핸들 드래그 (버텍스에 딱 붙음)

스냅 단위 조절: Edit → Grid and Snap Settings

Transform 빠른 초기화: 인스펙터 Transform 우클릭 → Reset

하이라키 / 프로젝트 창

이름 바꾸기: F2

빈 오브젝트 만들기: Ctrl+Shift+N

트리 한 번에 접기/펼치기: 하이라키에서 Alt + 폴드아웃(화살표) 클릭

프로젝트 검색 필터 (자주씀):

타입: t:Prefab, t:Material, t:Script, t:Scene

라벨: l:라벨이름

이름 키워드: 그대로 입력

인스펙터 & 컴포넌트 꿀팁

컴포넌트 값 복붙: 톱니 → Copy Component / 대상에서 Paste Component Values

같은 타입 컴포넌트 통째로 붙이기: Paste Component As New

여러 컴포넌트 접기/펼치기: 컴포넌트 제목의 삼각형 Alt+클릭

인스펙터 잠금(고정): 인스펙터 상단 자물쇠 클릭

멀티 오브젝트 편집: 여러 개 선택 후 공통 프로퍼티 일괄 수정

드래그&드롭 참조: 하이라키 오브젝트를 인스펙터 필드로 끌어다 놓기

프리팹 워크플로우

프리팹 열기(격리 모드): 프리팹 더블클릭

인스턴스 변경 관리: 인스펙터 상단 Overrides → Apply / Revert

공통 변경은 원본 프리팹에서, 개별 변경은 Overrides로 관리

콘솔 & 디버깅

오류 시 재생 자동 일시정지: 콘솔의 Error Pause

재생 시 로그 자동 클리어: Clear on Play

로그 더블클릭: 해당 스크립트/라인으로 점프

오브젝트 연결 로그: Debug.Log("msg", gameObject) → 클릭 시 하이라키에서 하이라이트

카메라/씬 보기

카메라를 뷰에 맞추기: Ctrl+Shift+F

Gizmos 아이콘 크기/토글: 씬 뷰 상단 Gizmos에서 조절

2D/3D 전환: 씬 뷰 상단 2D 버튼

빌드/성능/안전 설정 팁

Playmode Tint 바꾸기(플레이 중 편집 실수 방지):
Edit → Preferences → Colors → Playmode tint

Enter Play Mode Options(도메인 리로드 생략으로 빠른 재생):
Project Settings → Editor → Enter Play Mode Options
(정적 상태 초기화 이슈 주의)

Profiler: Window → Analysis → Profiler (CPU/GPU/메모리, Deep Profile는 꼭 필요할 때만)

타겟 플랫폼 모듈(예: WebGL/Android): Unity Hub → Installs → Add modules
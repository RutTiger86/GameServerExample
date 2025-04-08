# 🚀 GameServerExample

## 📖 프로젝트 소개

**GameServerExample** 프로젝트는 게임 서버 구조를 직접 설계하고 구현하기 위한 **학습 및 포트폴리오 프로젝트**입니다.  
특히 **멀티스레딩**, **Socket 통신**, **서버 구조 설계**에 대한 이해를 심화하는 것을 목적으로 하고 있습니다.  

본 프로젝트는 실제 게임 서버의 **기본 아키텍처**와 유사하게 구성되어 있으며, 서버 간 통신, 데이터베이스 연동, 멀티스레드 환경 등을 직접 다루고 있습니다.

---

## ✅ 프로젝트 목표

- **게임 서버 구조**에 대한 깊은 이해
- **멀티스레딩** 및 **비동기 소켓 통신** 학습
- **서버 간 통신 (Socket 기반)** 설계 및 구현
- **독립적인 서버 모듈 구성** 및 **DI (의존성 주입)** 패턴 적용
- **Log4Net을 통한 통합 로깅 시스템** 적용
- **.NET 8 기반 고성능 서버 개발** 실습
- **Protobuf**를 이용한 경량화된 프로토콜 학습

---

## 🏗️ 프로젝트 구조

```  
GameServerExample  
│  
├── AuthServer                # 인증 서버 (로그인, 캐릭터 생성 등)  
├── AuthDBServer             # 인증 DB 연동 서버  
├── WorldServer               # 게임 월드 서버 (게임 플레이, 캐릭터 관리)  
├── WorldDBServer             # 월드 DB 연동 서버  
│  
├── Commons                  # 모든 서버에서 공용 사용  
│   ├── Server.Core          # Listener, Session, PacketSession 등 네트워크 핵심 모듈  
│   └── Server.Utill          # 각종 헬퍼   
│ 
├── Tools                  # 모든 서버에서 공용 사용  
│   ├── DummyClient          # 테스트 클라이언트
│   └── PacketGenerator          # Proto 및 코드 자동화   
│  
└── DataBase                 # 데이터베이스 관련  
    ├── AuthDB              # 인증 관련 DB  
    └── WorldDB             # 게임 월드 관련 DB  
```

---

## 🔗 서버 구성 및 역할

| 서버               | 역할                                                         |
|------------------|------------------------------------------------------------|
| **AuthServer**      | 사용자 **인증, 로그인, 캐릭터 생성/조회** 처리                          |
| **AuthDBServer**   | **AuthServer와 연동**, 인증/계정 관련 DB 처리 전담                      |
| **WorldServer**    | 실제 게임 **월드 관리, 캐릭터 이동, 인게임 기능** 처리                   |
| **WorldDBServer**  | **WorldServer와 연동**, 게임 월드 데이터(DB) 처리 전담                   |
| **Server.Core**    | **Listener, Session, PacketSession** 등 기본 네트워크 처리 모듈           |
| **Server.Util**    | **SessionManager, Log4NetFactory** 등 기본 헬퍼 모듈           |
| **DataBase(폴더)**   | 실제 서비스 데이터베이스 (AuthDB, WorldDB) 구성 및 테이블 정의              |
| **DummyClient**    | 테스트 클라이언트              |
| **PacketGenerator**  | Proto 및 Packet 관련 자동화 Generator              |

---

## ⚙️ 기술 스택

| 항목                       | 사용 기술                                     |
|--------------------------|--------------------------------------------|
| **언어**                   | C# (.NET 8)                                 |
| **네트워크 통신**            | **Socket (TCP)** 기반 직접 통신                    |
| **멀티스레드 처리**           | **ThreadPool**, **Task**, **Async/Await**          |
| **프로토콜**                | **Protobuf** (Google Protocol Buffers)         |
| **로깅 시스템**              | **Log4Net**                                 |
| **DI (의존성 주입)**         | **Microsoft.Extensions.DependencyInjection** |
| **설정 파일**               | **JSON 기반 환경설정** (서버별 독립 설정)         |

---

## 📦 사용 기술 특징 및 이유

| 기술                         | 이유 및 목적                                          |
|----------------------------|--------------------------------------------------|
| **Socket 직접 사용**             | 게임 서버의 **고성능 저수준 통신** 경험                                 |
| **ArrayPool<byte> 활용 (예정)** | **Buffer 재사용**을 통한 메모리 최적화, 대규모 클라이언트 대응                    |
| **멀티 Accept / 세션 관리**     | 대규모 동접 사용자 처리 위한 고성능 Accept 구조                          |
| **Protobuf**                  | **가볍고 빠른 이진 프로토콜** 학습 및 적용                                 |
| **Log4Net**                   | 파일 + 콘솔 동시 로깅, 운영 로그 관리                                   |
| **DI 적용**                   | **모듈화**, **유지보수성 향상**, **테스트 용이성** 강화                      |
| **JSON 설정 파일**             | 서버별 **유연한 설정** 관리                                         |

---

## 🚧 현재 진행 상황

| 모듈                      | 진행 상황         |
|-----------------------|---------------|
| **AuthServer**         | ✅ 개발 중 (ID/PW Login 추가 완료 ) |
| **AuthDBServer**       | ✅ 개발 중 (DB 조회 기능 추가 완료 ) |
| **WorldServer**        | ⏳ 미구현 (계획 예정)       |
| **WorldDBServer**      | ⏳ 미구현 (계획 예정)       |
| **Server.Core**        | ✅ 개발 중 (Listener, Session 및 TLS 적용완료) |
| **Server.Util**        | ✅ 개발 중 (Session Manager 및 기타 Utill 추가중) |
| **AuthDB**             | ✅ 개발 중 (Account테이블 구성완료 Migration 추가 완료 )   |
| **WorldDB**            | ⏳ 미구현 (향후 작업 예정)   |
| **DummyClient**        | ✅ 개발 중 (Client-Auth 연결 및 Login 구성완료)   |
| **PacketGenerator**    | ✅ 개발 중 (자동화 기능 추가, Proto 생성 구성 완료)  |

---

## 📚 개발 목적

> **"게임 서버 개발에 대한 전반적인 이해를 쌓고, 멀티스레드 환경에 대한 학습을 목적으로 시작한 프로젝트입니다."**  
> 실제 상용 서버 구조에 근접한 시스템을 직접 설계/구현하며 게임 서버의 동작 원리를 깊이 이해하기 위한 과정입니다.

---

## ✍️ 사용 방법 (예정)

- **.NET 8 SDK 설치**
- 서버별 **json 설정 파일 작성** (ex: `authserver.json`, `worldserver.json`)
- 각 서버 실행:
\`\`\`bash
dotnet run --project AuthServer
\`\`\`

---

# ⚠️ 테스트용 인증서 안내

이 저장소에 포함된 `mycert.pfx`는 로컬 테스트용 TLS 인증서입니다.

- CN: `127.0.0.1`
- 비밀번호: `1234`
- 보안성이 없으며, 실제 서비스에 절대 사용하지 마세요.
- 외부 공개용 TLS 인증서가 필요하다면 정식 CA 또는 Let's Encrypt를 사용하세요.

## 🚀 한 줄 요약
"게임 서버 개발의 처음부터 끝까지, 직접 설계하고 배워가는 고성능 게임 서버 아키텍처 프로젝트!"

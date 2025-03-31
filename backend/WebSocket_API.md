# SignalR WebSocket API dokumentáció

## Áttekintés

Ez a dokumentum bemutatja a játékhoz tartozó WebSocket API-t, amely SignalR segítségével van implementálva a Backend projektben.

## Kapcsolódás

A WebSocket kapcsolat létrehozásához a klienseknek hitelesítettnek kell lenniük. A kapcsolatot a következő URL-en keresztül lehet kezdeményezni:

```
ws://<szerver>/game
```

### Hitelesítés

- A WebSocket kapcsolat hitelesítést igényel, ezért meg kell adni egy **access tokent**.
- A kliensnek az access tokent **sütiként** kell továbbítania `access_token` néven.

## Meghívható események

### **JoinGame**

Lehetővé teszi a felhasználó számára, hogy csatlakozzon egy játékhoz egy adott játékkóddal.

#### **Paraméterek**

- **`gameCode` (string)** – az adott játékot azonosító játékkód

#### **Válasz események**

- **`JoinSuccess`** – ha a felhasználó sikeresen csatlakozott egy játékhoz

- **`JoinFailed`** – ha a felhasználó nem tudott csatlakozni a játékhoz

## Csoportok

- Azok a felhasználók, akik sikeresen csatlakoznak egy játékhoz, hozzáadódnak egy SignalR csoporthoz, amelyet a `gameCode` azonosít.
- Ez lehetővé teszi, hogy a játékspecifikus eseményekről csak az adott játékban lévő felhasználók értesüljenek.

## Példa WebSocket használatra kliens oldalon (React)

```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/game", {
    withCredentials: true,
  })
  .withAutomaticReconnect()
  .build();

connection.start().then(() => {
  console.log("WebSocket kapcsolat létrejött.");
  connection.invoke("JoinGame", "abcd1234");
});

connection.on("JoinSuccess", () => {
  console.log("Sikeresen csatlakoztál a játékhoz.");
});

connection.on("JoinFailed", () => {
  console.log("Nem sikerült csatlakozni a játékhoz.");
});
```

## Jövőbeli bővítési lehetőségek

- további WebSocket metódusok a játékesemények kezeléséhez
- hibakezelés és újracsatlakozási stratégiák

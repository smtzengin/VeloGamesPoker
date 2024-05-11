# Texas Hold'em Poker

This project is developed based on the Offline Texas Hold'em Poker, which is the third project provided by Velo Games. Our game features login, registration, auto-login, leaderboard, battling against bots, and gameplay adhering to all Texas Hold'em poker rules. The experience points (exp) and scores earned by the user in the game are stored on Firebase.

Before starting: After downloading the project, it is necessary to install the following package before launching the game.

## Opponent's Decision Formula
| **Before Pre-Flop** | 
| :-------- | 
Aggression * Mathf.Log((HandCardPoint() + OnePairPoint()) / 2f, 2) * Randomness * rand / (Caution + Stupidity)

| **After Pre-Flop** | 
| :-------- | 
Aggression * Mathf.Log((HandCardPoint() + (int)_bestHand) / 2f, 2) * Randomness * rand / (Caution + Stupidity)

### Explaining Of Symbols
|   Name   |    Describe   |
|----------|:-------------:|
| Aggression | Bot's Aggressiveness means how strong bot continue playing even with bad hand. |
| Randomness | Defines how strong bot should took random actions. |
| Caution | It determines how strong bot play cautious with bad hand. | 
| Stupidity | It determines how strong bot can make stupid decisions. | 
| HandCardPoint() | Calculates Bot's hand and return result. | 
| OnePairPoint() | Checks if Bot's has One Pair and returns result 0 or One Pair points. | 
| _bestHand | It is a field which returns Bot's best combination with it's own hand and the opened cards on the table. |

## Installation

Firebase Auth Package

```bash
  https://file.io/mSLXO6jilKMu
```
# Contributions
| Contributions          | Link |
| ----------------- | ------------------------------- |
| Eyüp Han Kaygusuz | https://github.com/eyuphankygsz |
| Batu Sarihan | https://github.com/bbatus |
| Samet Zengin | https://github.com/smtzengin |
| Akif Kahraman | https://github.com/akifpsh |

# Demo Video
| **Demo Video** | 
| :-------- |
[![Game Video](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/0d511474-cf1a-45e9-8182-592fe68d403c)](https://www.youtube.com/watch?v=NFL9X__OKtQ")

# Screenshots
| **Login Screen** | 
| :-------- | 
| ![Screenshot 2024-05-10 120306](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/924bd8f1-9459-473c-acd1-2ef24739ee42) |

|**Register Screen**|
|:-------------|
|![image](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/945bd22f-aab1-4dd5-b5ab-431826401065)|

|**Loading Screen**|
|:-------------|
|![image](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/f3dbcb21-b19a-4f91-814f-89cdfb00bc2b)|

|**MainMenu Screen** |
|:-------------|
|![image](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/12de82e7-d254-4847-b535-82b6d7eeea9d)|

|**Leaderboard Screen**|
|:-------------|
|![image](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/183245e7-619f-48ee-bd1e-560be74f5086)|

|**Settings Screen**|
|:-------------|
[<img src="https://github.com/smtzengin/VeloGamesPoker/assets/73519045/fb718caa-36f4-46b7-b86e-571655c12bce" width="100%">](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/7f535581-c0a3-44f4-ac82-dce199e62193")

|**In-Game **|
|:-------------|
|![image](https://github.com/smtzengin/VeloGamesPoker/assets/73519045/0d511474-cf1a-45e9-8182-592fe68d403c)|
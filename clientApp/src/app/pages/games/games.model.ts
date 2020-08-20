export class GameInfo {
    gameID: number;
    gameName: string;
    winValue1: number;
    winValue2: number;
    winValue3: number;
    winValue4: number;
    winValue5: number;
    winValue6: number;
    isDeleted: boolean;
}
export class GameDetailInfo {
    gameDetailID: number;
    gameID: number;
    valueOfRow1: number;
    valueOfRow2: number;
    valueOfRow3: number;
    valueOfRow4: number;
    valueOfRow5: number;
    valueOfRow6: number;
}
export class GameCompleteInfo {
    gameInfo: GameInfo;
    gameDetail: GameDetailInfo;
}
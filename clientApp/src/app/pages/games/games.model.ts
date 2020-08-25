export class GameInfo {
    gameID: number;
    gameName: string;
    winValue1: number;
    winValue2: number;
    winValue3: number;
    winValue4: number;
    winValue5: number;
    winValue6: number;
    winValueList: any;
    isDeleted: boolean;
}
export class GameValues {
    gameValueID: number;
    rowNum1: number;
    rowNum2: number;
    rowNum3: number;
    rowNum4: number;
    rowNum5: number;
    rowNum6: number;
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
export class UserGame {
    userGameID: number;
    userID: number;
    gameID: number;
    value: number;
}
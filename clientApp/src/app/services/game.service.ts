import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { GameInfo, GameCompleteInfo, UserGame, GameValues } from '../pages/games/games.model';

@Injectable({ providedIn: 'root' })
export class GameService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<GameInfo[]>(`${environment.apiUrl}/api/GamesInfo`);
    }
    getLastWinner() {
        return this.http.get<GameInfo>(`${environment.apiUrl}/api/GamesInfo/GetLastWinner`);
    }
    getGameValues() {
        return this.http.get<GameValues[]>(`${environment.apiUrl}/api/UserGame/GetGameValues`);
    }
    postGame(gameInfo: any) {
        return this.http.post(`${environment.apiUrl}/api/GamesInfo`, gameInfo);
    }

    updateGame(gameInfo: any, id: any) {
        return this.http.put(`${environment.apiUrl}/api/GamesInfo/` + id, gameInfo);
    }

    deleteGame(id: number) {
        return this.http.delete(`${environment.apiUrl}/api/GamesInfo/${id}`);
    }
    getGameDetails(gameID) {
        return this.http.get<GameCompleteInfo[]>(`${environment.apiUrl}/api/GamesInfo/` + gameID);
    }



    StartGame(gameID: number, value: number, isConfirmed: boolean) {
        var id = localStorage.getItem('id');
        var userid: number = +id;
        var userGameObject = {
            userGameID: -1,
            userID: userid,
            gameID: gameID,
            value: value,
            isConfirmed: isConfirmed
        };
        return this.http.post(`${environment.apiUrl}/api/UserGame`, userGameObject);
    }
    GetUserGameValues(gameID: number) {
        var id = localStorage.getItem('id');
        var userid: number = +id;
        var userGameObject = {
            userID: userid,
            gameID: gameID,
        };
        return this.http.post(`${environment.apiUrl}/api/UserGame`, userGameObject);
    }
    GetGameValues(gameID: number) {
        return this.http.post(`${environment.apiUrl}/api/UserGame` + gameID, {});
    }
    UpdateWinner(gameID: number) {
        var id = localStorage.getItem('id');
        var userid: number = +id;
        var userGameObject = {
            userGameID: -1,
            userID: userid,
            gameID: gameID,
            value: 0,
            isConfirmed: true
        };
        return this.http.post(`${environment.apiUrl}/api/GamesInfo/UpdateWinner`, userGameObject);
    }


    /** New changes */
    InsertSelectedColValue(objColDetailWithRows: any) {
        return this.http.post(`${environment.apiUrl}/api/UserGame`, objColDetailWithRows);
    }
    bulkRemoveValueFromGame(gameID: number, notConfirmedValues: any) {
        var id = localStorage.getItem('id');
        var userid: number = +id;
        var finalConfirmedValues = [];
        for (var i = 0; i < notConfirmedValues.length; i++) {
            var userGameObject = {
                userGameID: -1,
                userID: userid,
                gameID: gameID,
                gameValueID: notConfirmedValues[i],
                isConfirmed: false,
                isDeleted: true
            };
            finalConfirmedValues.push(userGameObject);
        }

        return this.http.post(`${environment.apiUrl}/api/UserGame/ConfirmedUserGameValues`, finalConfirmedValues);
    }
    confirmValue(gameID: number, confirmedValues: any) {
        var id = localStorage.getItem('id');
        var userid: number = +id;
        var finalConfirmedValues = [];
        for (var i = 0; i < confirmedValues.length; i++) {
            var userGameObject = {
                userGameID: -1,
                userID: userid,
                gameID: gameID,
                gameValueID: confirmedValues[i],
                isConfirmed: true
            };
            finalConfirmedValues.push(userGameObject);
        }
        return this.http.post(`${environment.apiUrl}/api/UserGame/ConfirmedUserGameValues`, finalConfirmedValues);
    }
    UpdateGameStatus(gameInfo: any) {
        return this.http.post(`${environment.apiUrl}/api/UserGame/UpdateGameStatus`, gameInfo);
    }
    UpdateWinnigValues(gameInfo: any) {
        return this.http.post(`${environment.apiUrl}/api/GamesInfo/UpdateWinnigValues`, gameInfo);
    }
    InsertUserOfGame(userOfGame: any) {
        return this.http.post(`${environment.apiUrl}/api/GamesInfo/InsertUserOfGame`, userOfGame);
    }
}
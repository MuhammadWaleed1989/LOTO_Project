import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { GameInfo, GameCompleteInfo, UserGame } from '../pages/games/games.model';

@Injectable({ providedIn: 'root' })
export class GameService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<GameInfo[]>(`${environment.apiUrl}/api/GamesInfo`);
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
    ConfirmValue(gameID: number, confirmedValues: any) {
        var id = localStorage.getItem('id');
        var userid: number = +id;
        var finalConfirmedValues = [];
        for (var i = 0; i < confirmedValues.length; i++) {
            var userGameObject = {
                userGameID: -1,
                userID: userid,
                gameID: gameID,
                value: confirmedValues[i],
                isConfirmed: true
            };
            finalConfirmedValues.push(userGameObject);
        }

        return this.http.post(`${environment.apiUrl}/api/UserGame/ConfirmedUserGameValues`, finalConfirmedValues);
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
}
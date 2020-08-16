import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { GameInfo } from '../pages/games/games.model';

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
}
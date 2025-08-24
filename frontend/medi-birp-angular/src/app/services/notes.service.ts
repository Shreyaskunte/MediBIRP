import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateNoteDto {
  patientId: string;
  clinicianId: string;
  rawText: string;
  noteDate: string; // ISO string
}

export interface Note {
  id: number | string;
  patientId: string;
  clinicianId: string;
  rawText: string;
  createdAt: string; // ISO
}

const NOTE_SERVICE_BASE = 'http://localhost:5002'; // 🔁 change if your NoteService runs elsewhere

@Injectable({ providedIn: 'root' })
export class NotesService {
  private baseUrl = `${NOTE_SERVICE_BASE}/api/notes`;

  constructor(private http: HttpClient) {}

  getNotes(): Observable<Note[]> {
    return this.http.get<Note[]>(this.baseUrl);
  }

  createNote(dto: CreateNoteDto): Observable<Note> {
    return this.http.post<Note>(this.baseUrl, dto);
  }

  exportPdf(id: number | string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, { responseType: 'blob' });
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { NotesService } from '../services/notes.service';

@Component({
  selector: 'app-note-list',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './note-list.component.html'
})
export class NoteListComponent implements OnInit {
  notes: any[] = [];
  loading = false;

  constructor(private notesService: NotesService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading = true;
    this.notesService.getNotes().subscribe({
      next: (data) => {
        this.notes = data;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  exportPdf(note: any) {
    alert('PDF export not implemented, noteId: ' + note.id);
  }
}

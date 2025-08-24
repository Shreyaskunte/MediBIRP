import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NotesService } from '../services/notes.service';

@Component({
  selector: 'app-note-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './note-create.component.html'
})
export class NoteCreateComponent {
  patientId = '';
  rawText = '';
  submitting = false;   // ✅ add this, was missing

  constructor(private notes: NotesService) {}

  submit() {
    this.submitting = true;
    const dto = {
      patientId: this.patientId,
      clinicianId: 'clinician-1',
      rawText: this.rawText,
      noteDate: new Date().toISOString() // ✅ must be string, not Date
    };
    this.notes.createNote(dto).subscribe({
      next: () => {
        alert('Note created');
        this.submitting = false;
      },
      error: () => (this.submitting = false)
    });
  }
}

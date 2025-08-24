import { Routes } from '@angular/router';
import { NoteCreateComponent } from './note-create/note-create.component';
import { NoteListComponent } from './note-list/note-list.component';

export const routes: Routes = [
  { path: '', component: NoteListComponent },      // default route
  { path: 'create', component: NoteCreateComponent }
];

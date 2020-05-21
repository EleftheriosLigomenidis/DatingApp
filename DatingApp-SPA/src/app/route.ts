import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditResolver } from './resolvers/member-edit.resolver';
import { MemberDetailResolver } from './resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import {MemberListResolver} from  './resolvers/member-detail.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';

export const appRoutes: Routes = [
{path: '', component: HomeComponent},
{
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
        {path: 'members', component: MemberListComponent,resolve: {users:MemberListResolver}},
        {path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver}},
        {path: 'messages', component: MessagesComponent},
        {path: 'member/edit', component: MemberEditComponent, resolve:{user : MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]},
        {path: 'lists', component: ListsComponent},

    ]
},


{path: '**', redirectTo: '', pathMatch: 'full'},

];
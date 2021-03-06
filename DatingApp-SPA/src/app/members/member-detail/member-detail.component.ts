import { Component, OnInit, ViewChild } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery-9';
import { TabsetComponent } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  constructor(private userService: UserService, private alertify: AlertifyService,
              private route: ActivatedRoute) { }
              @ViewChild('membertabs', {static: true})
              memberTabs: TabsetComponent;
user: User;
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];
  ngOnInit() {
   this.route.data.subscribe(data => {
     this.user = data.user;
     this.route.queryParams.subscribe(params => {
  const selectedTab = params.tab;
  this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;

});


   });

   this.galleryOptions = [
    {
      width: '500px',
      height: '500px',
      imagePercent: 100,
      thumbnailsColumns: 4,
      imageAnimation: NgxGalleryAnimation.Slide,
      preview: false
    }
  ];
   this.galleryImages = this.getImages();
  }

// members/4
  loaduser(){

    this.userService.getUser(this.route.snapshot.params.id).subscribe((user: User) => {
      this.user = user;

    }, error => { this.alertify.error(error); });
  }




  getImages(){
    const imagesUrls = [];
    for (const photo of this.user.photos){
      imagesUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      });
    }
    return imagesUrls;
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }
}

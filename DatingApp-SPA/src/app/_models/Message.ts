export interface Message {
    id:number;
    senderId:number;
    senderKnownAs:string;
    senderPhotoUrl:string;
    recipientId:number;
    recipientKnownAs:string;
    recipientPhotoUrl:string;
    content:string;
    isRead:Boolean;
    dateRead:Date;
    messageSent:Date;

}

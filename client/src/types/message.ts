export type Message ={
  id: string
  content: string
  dateSent: string
  senderId: string
  senderDisplayName: string
  senderImageUrl?: string
  recipientId: string
  recipientDisplayName: string
  recipientImageUrl?: string
  dateRead?: string
  currentUserSender?:boolean
}
import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
  protected imageUrl =signal<string | ArrayBuffer | null>(null) ; // για την προεπισκόπηση της εικόνας
  protected isDragging =false; // για να δείχνει αν γίνεται drag over 
  protected fileToUpload:File | null =null;
  uploadFile=output<File>();// στελνει το αρχειο στον γονέα για αποστολή
  loading=input<boolean>(false);// την αποστολή την κάνει ο γονέας αλλα το παιδι δείχνει αν φορτώνει

  onDragOver(event:DragEvent){
    event.preventDefault();// αποτρέπει το default behavior του browser δλδ να ανοίξει το αρχείο σε αλλη καρτέλα
    event.stopPropagation();// αποτρέπει την διάδοση του event σε γονείς
    this.isDragging=true;
  }

  onDragLeave(event:DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragging=false;
  }

  onDrop(event:DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragging=false;

    if(event.dataTransfer?.files.length){
      //για να πάρω μονο μια εικόνα
      this.fileToUpload=event.dataTransfer.files[0];
      this.previewImage(this.fileToUpload);
    }
  }

  onCancel(){
    this.imageUrl.set(null);
    this.fileToUpload=null;
  }

  onUploadFile(){
    if(this.fileToUpload){
   this.uploadFile.emit(this.fileToUpload);
  }
  }

  //Το FileReader χρησιμοποιείται μόνο για client-side επεξεργασία
  // Δεν αντικαθιστά το upload στο backend
  private previewImage(file: File){
    const reader=new FileReader();// για να διαβάσω το αρχείο ως data URL και οχι ως binary 
    reader.onload=(e) =>this.imageUrl.set(e.target?.result);// όταν ολοκληρωθεί η ανάγνωση, ενημερώνω το signal με το αποτέλεσμα
    reader.readAsDataURL(file);// διαβάζω το αρχείο ως data URL
  }
}

import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
  protected imageUrl =signal<string | ArrayBuffer | null>(null) ;
  protected isDragging =false;
  protected fileToUpload:File | null =null;
  uploadFile=output<File>();
  loading=input<boolean>(false);

  onDragOver(event:DragEvent){
    event.preventDefault();
    event.stopPropagation();
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

  private previewImage(file: File){
    const reader=new FileReader();
    reader.onload=(e) =>this.imageUrl.set(e.target?.result);
    reader.readAsDataURL(file);
  }
}

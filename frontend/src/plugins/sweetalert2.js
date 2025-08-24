import Swal from 'sweetalert2';

export const Toast = Swal.mixin({
    toast: true,
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    position: 'top',
    theme: 'auto',
});


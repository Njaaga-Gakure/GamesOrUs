window.ShowMessage = (type, message) => {
    if (type === 'success') {
        toastr.success(message, "Operation was Success!")
    }

    if (type === 'error') {
        toastr.error(message, "Operation was Failed!")
    }
}
function ToastMessage(heading, message, icon, bgColor) {
    $.toast({
        heading: heading,
        text: message,
        showHideTransition: 'slide',
        icon: icon,
        hideAfter: 5000,
        position: 'top-center',
        stack: false,
        loader: false,
        textAlign: 'center',
        bgColor: bgColor,
        textColor: 'white'
    })
}
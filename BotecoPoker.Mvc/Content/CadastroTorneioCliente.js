$('#meuForm').ready(function () {
    OpenModalValue();
});

function OpenModalValue() {
    var teste = $('#abreModal').val();
    if (teste == '666') {
        OpenModal();
    }
};

function OpenModal() {
    $('#ModalBuscaCliente').modal({
        show: true
    });
}

//$('#codigoCliente1').blur(function () {
//    debugger;
//    var codigo = $('#codigoCliente1').val();




//    $('#CodigoCliente2').val(codigo); 

//    var val = $('#CodigoCliente2').val();

//    $('#MeuSubmit').click();

//});

$('#codigoCliente1').blur(function () {
    debugger;
    var codigo = $('#codigoCliente1').val();

    if (codigo.trim() == "")
        return;

    var input = '<input type="text" name="Codigo" ' + "value='" + codigo + "'" + ' hidden id="CodigoCliente2" /> ';

    $('#inserirForm').append(input);

    $('#MeuSubmit').click();
});

/////// <reference path="maskmoney.min.js" />
////$('#Dinheiro').keyup(function () {
////    debugger;
////    var tamanho = $("#Dinheiro").val().length;
////    if (tamanho >= 2) {
////        $("#Dinheiro").maskMoney();
////    }
////});

function CalculaTroco(saldo) {
    debugger;
    var valorTotal = document.getElementById("ValorTotal").value;
    //valorTotal = valorTotal.replace(',00', '');
    valorTotal = valorTotal.replace(',', '.');
    var dinheiro = document.getElementById("Dinheiro").value;
    if (dinheiro == '')
        dinheiro = '0';
    //dinheiro = dinheiro.replace(',00', '');
    dinheiro = dinheiro.replace(',', '.');
    dinheiro = (parseFloat(saldo) + parseFloat(dinheiro));
    if (dinheiro != valorTotal) {
        var ValTroco = (parseFloat(dinheiro) - parseFloat(valorTotal));
        var test = document.getElementById("teste");
      

        if (ValTroco > 0) {
            test.innerText = ValTroco;
            test.innerText = "Troco: R$ " + test.innerText.replace('.', ',');
            var trocoSaldo = document.getElementById("TrocoSaldo");
            trocoSaldo.removeAttribute("hidden");
        }
        else {
            test.innerText = "";
            trocoSaldo = document.getElementById("TrocoSaldo");
            trocoSaldo.setAttribute("hidden", "hidden");
        }
    }
    if (dinheiro == valorTotal) {
        var div = document.getElementById("teste");
        div.innerHTML = "";
        trocoSaldo = document.getElementById("TrocoSaldo");
        trocoSaldo.setAttribute("hidden", "hidden");
    }
}


//$('#Dinheiro2').keyup(function () {
//    debugger;
//    var tamanho = $("#Dinheiro2").val().length;
//    if (tamanho >= 2) {
//        $("#Dinheiro2").maskMoney();
//    }
//});

function CalculaTroco2(saldo) {
    debugger;
    var valorTotal = document.getElementById("ValorTotal2").value;
    //valorTotal = valorTotal.replace(',00', '');
    valorTotal = valorTotal.replace(',', '.');
    var dinheiro = document.getElementById("Dinheiro2").value;
    if (dinheiro == '')
        dinheiro = '0';
    //dinheiro = dinheiro.replace(',00', '');
    dinheiro = dinheiro.replace(',', '.');
    dinheiro = (parseFloat(saldo) + parseFloat(dinheiro));
    if (dinheiro != valorTotal) {
        var ValTroco = (parseFloat(dinheiro) - parseFloat(valorTotal));
        var test = document.getElementById("teste2");

        if (ValTroco > 0) {
            test.innerText = ValTroco;
            test.innerText = "Troco: R$ " + test.innerText.replace('.', ',');
            var trocoSaldo = document.getElementById("TrocoSaldo2");
            trocoSaldo.removeAttribute("hidden");
        }
        else {
            test.innerText = "";
            trocoSaldo = document.getElementById("TrocoSaldo2");
            trocoSaldo.setAttribute("hidden", "hidden");
        }
    }
    if (dinheiro == valorTotal) {
        var div = document.getElementById("teste2");
        div.innerHTML = "";
        trocoSaldo = document.getElementById("TrocoSaldo2");
        trocoSaldo.setAttribute("hidden", "hidden");
    }
}

function chamaNoLoad1(valor) {
    debugger;
    CalculaTroco(valor);
}

function chamaNoLoad2(valor) {
    debugger;
    CalculaTroco2(valor);
}



const container = document.querySelector(".container");
const seats = document.querySelectorAll(".row .seat:not(.sold)");
const count = document.getElementById("count");
const total = document.getElementById("total");
const movieSelect = document.getElementById("movie");

populateUI();

let ticketPrice = +movieSelect.value;

// Save selected movie index and price
function setMovieData(movieIndex, moviePrice) {
    localStorage.setItem("selectedMovieIndex", movieIndex);
    localStorage.setItem("selectedMoviePrice", moviePrice);
}
const vipSeatMoney = 70000;
const normalSeatMoney = 50000;
// Update total and count
function updateSelectedCount() {
    const selectedSeats = document.querySelectorAll(".row .seat.selected");
    const seatsIndex = [...selectedSeats].map((seat) => [...seats].indexOf(seat));

    localStorage.setItem("selectedSeats", JSON.stringify(seatsIndex));

    const selectedSeatsCount = selectedSeats.length;
    const selectedSeatText = [];
    let seatselect;
    selectedSeats.forEach((seat) => {
        selectedSeatText.push(seat.textContent);
    });
    if (seatselect === null) {
        count.innerText = '';
    } else count.innerText = selectedSeatText.join(",");


    let totalPrice = 0;
    selectedSeats.forEach((seat) => {
        if (seat.classList.contains("vip")) {
            totalPrice += vipSeatMoney; // Nếu ghế được chọn là ghế VIP, thêm 50,000 đồng vào tổng giá tiền
        } else {
            totalPrice += normalSeatMoney;
        }
    });
    total.innerText = totalPrice;

    setMovieData(movieSelect.selectedIndex, movieSelect.value);
}


// Get data from localstorage and populate UI
function populateUI() {
    const selectedSeats = JSON.parse(localStorage.getItem("selectedSeats"));

    if (selectedSeats !== null && selectedSeats.length > 0) {
        seats.forEach((seat, index) => {
            if (selectedSeats.indexOf(index) > -1) {
                console.log(seat.classList.add("selected"));
            }
        });
    }

    const selectedMovieIndex = localStorage.getItem("selectedMovieIndex");

    if (selectedMovieIndex !== null) {
        movieSelect.selectedIndex = selectedMovieIndex;
        console.log(selectedMovieIndex)
    }
}
console.log(populateUI())
    // Movie select event
movieSelect.addEventListener("change", (e) => {
    ticketPrice = +e.target.value;
    setMovieData(e.target.selectedIndex, e.target.value);
    updateSelectedCount();
});

// Seat click event
container.addEventListener("click", (e) => {
    if (
        e.target.classList.contains("seat") &&
        !e.target.classList.contains("sold")
    ) {
        e.target.classList.toggle("selected");

        updateSelectedCount();
    }
});

// Initial count and total set
updateSelectedCount();

function addRowToContainer(containerSelector, rows, cols) {
    const container = document.querySelector(containerSelector);
    //add row to container
    for (let i = 0; i < rows; i++) {
        const row = document.createElement('div');
        row.classList.add('row');
        for (let j = 0; j < cols; j++) {
            const seat = document.createElement('div');
            seat.classList.add('seat');
            const char = String.fromCharCode(i + 65);
            if (i >= 0 && i < rows / 4) {
                seat.classList.toggle('normal');
            } else if (i > 3 * rows / 4 && i < rows) {
                seat.classList.toggle('normal');
            } else {
                seat.classList.toggle('vip');
            }
            let idSeat;
            if (j < 9) {
                let num = (j + 1);
                let str = num.toString();
                idSeat = char + '0' + str;
                seat.textContent = idSeat;
            } else {
                let num = (j + 1);
                let str = num.toString();
                idSeat = char + str;
                seat.textContent = idSeat;
            }
            seat.classList.toggle(idSeat);
            row.appendChild(seat);

        }
        container.appendChild(row);
    }
}

buySeat();

function buySeat() {
    const selectedSeats = document.querySelectorAll(".row .seat.selected");
    selectedSeats.forEach((seat) => {
        seat.classList.replace('selected', 'sold');
    });
}
ResetSeat();

function ResetSeat() {
    const selectedSeats = document.querySelectorAll(".row .seat.sold");
    selectedSeats.forEach((seat) => {
        seat.classList.remove('sold');
    });
}
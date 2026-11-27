const family = document.querySelector('#family');
const order = document.querySelector('#order');
const body = document.querySelector('#result-rows');
const count = document.querySelector('#result-count');
const rows = Array.from(body.rows);

function updateResults() {
  const visible = rows.filter(row => row.cells[1].textContent.startsWith(family.value));
  const sortColumn = { accuracy: 3, auc: 4 }[order.value];

  if (sortColumn !== undefined) {
    visible.sort((first, second) =>
      parseFloat(second.cells[sortColumn].textContent) -
      parseFloat(first.cells[sortColumn].textContent)
    );
  }

  body.replaceChildren(...visible);
  count.textContent = `Showing ${visible.length} of ${rows.length} results.`;
}

family.addEventListener('change', updateResults);
order.addEventListener('change', updateResults);
updateResults();
document.querySelector('#filters').hidden = false;

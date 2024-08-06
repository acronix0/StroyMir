import lib.CircularLinkedList;

public class Main {
    public static void main(String[] args) {
        CircularLinkedList<Integer> intList = new CircularLinkedList<>(new Integer[]{1, 2, 3, 4, 5});
        System.out.println("Исходный список:  ");
        CircularLinkedList<Integer>.CircularListIterator iterator = intList.iterator();
        while (iterator.hasNext()) {
            System.out.print(iterator.next() + " ");
        }

        intList.add(6);
        System.out.println("\nСписок после добавления 6:  ");
        intList.print();

        intList.set(2, 10);
        System.out.println("\nСписок после изменения элемента на индексе 2 на 10:  ");
        intList.print();

        intList.remove(3);
        System.out.println("\nСписок после удаления элемента на индексе 3:  ");
        intList.print();
    }
}

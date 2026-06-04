import { useEffect, useState } from 'react';
import {
  Table, ScrollArea, Button, Group, TextInput, ActionIcon, Modal,
  NumberInput, Stack, Pagination, Title, Badge, Box, LoadingOverlay,
  Menu, Text, Alert
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconSearch, IconPlus, IconEdit, IconTrash, IconDotsVertical, IconBook, IconAlertCircle } from '@tabler/icons-react';
import axiosClient from '../api/apiClient';

interface Book {
  id: string;
  bookCode: string;
  title: string;
  author: string;
  category: string;
  publisher: string;
  publishYear: number;
  quantity: number;
  shelfLocation?: string;
  status: string;
}

export default function BookManagement() {
  const [books, setBooks] = useState<Book[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form states
  const [opened, { open, close }] = useDisclosure(false);
  const [editingBook, setEditingBook] = useState<Book | null>(null);
  const [title, setTitle] = useState('');
  const [author, setAuthor] = useState('');
  const [category, setCategory] = useState('');
  const [publisher, setPublisher] = useState('');
  const [publishYear, setPublishYear] = useState<number | ''>('');
  const [quantity, setQuantity] = useState<number>(1);
  const [shelfLocation, setShelfLocation] = useState('');
  const [bookCode, setBookCode] = useState('');

  // Fetch books from api
  const fetchBooks = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await axiosClient.get('/Books', {
        params: {
          search: search || undefined,
          pageNumber: page,
          pageSize: pageSize,
        },
      });
      if (res.data && res.data.success) {
        setBooks(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      } else {
        setError(res.data.message || 'Không thể tải danh sách sách.');
      }
    } catch (err: any) {
      console.error(err);
      setError('Đã xảy ra lỗi khi kết nối với máy chủ.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBooks();
  }, [page, search]);

  const handleOpenAdd = () => {
    setEditingBook(null);
    setTitle('');
    setAuthor('');
    setCategory('');
    setPublisher('');
    setPublishYear(new Date().getFullYear());
    setQuantity(1);
    setShelfLocation('');
    setBookCode('');
    open();
  };

  const handleOpenEdit = (book: Book) => {
    setEditingBook(book);
    setTitle(book.title);
    setAuthor(book.author);
    setCategory(book.category);
    setPublisher(book.publisher);
    setPublishYear(book.publishYear);
    setQuantity(book.quantity);
    setShelfLocation(book.shelfLocation || '');
    setBookCode(book.bookCode);
    open();
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const payload = {
        title,
        author,
        category,
        publisher,
        publishYear: Number(publishYear),
        quantity,
        shelfLocation: shelfLocation || null,
        bookCode: bookCode || null,
      };

      if (editingBook) {
        // Update Book
        const res = await axiosClient.put(`/Books/${editingBook.id}`, {
          id: editingBook.id,
          ...payload,
        });
        if (res.data && res.data.success) {
          close();
          fetchBooks();
        } else {
          setError(res.data.message || 'Lỗi cập nhật sách.');
        }
      } else {
        // Create Book
        const res = await axiosClient.post('/Books', payload);
        if (res.data && res.data.success) {
          close();
          setPage(1);
          fetchBooks();
        } else {
          setError(res.data.message || 'Lỗi thêm sách.');
        }
      }
    } catch (err: any) {
      console.error(err);
      const errMsg = err.response?.data?.message || err.message || 'Không thể lưu thông tin.';
      setError(errMsg);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Bạn có chắc chắn muốn xóa cuốn sách này không?')) return;
    setError(null);
    try {
      const res = await axiosClient.delete(`/Books/${id}`);
      if (res.data && res.data.success) {
        fetchBooks();
      } else {
        setError(res.data.message || 'Lỗi xóa sách.');
      }
    } catch (err: any) {
      console.error(err);
      setError('Lỗi khi thực hiện xóa sách.');
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  const rows = books.map((book) => (
    <Table.Tr key={book.id}>
      <Table.Td style={{ fontWeight: 600 }}>{book.bookCode}</Table.Td>
      <Table.Td>
        <Group gap="xs">
          <IconBook size="1rem" style={{ color: '#228be6' }} />
          <Text size="sm" fw={500}>{book.title}</Text>
        </Group>
      </Table.Td>
      <Table.Td>{book.author}</Table.Td>
      <Table.Td><Badge color="blue" variant="light">{book.category}</Badge></Table.Td>
      <Table.Td>{book.publisher} ({book.publishYear})</Table.Td>
      <Table.Td style={{ textAlign: 'center' }}>{book.quantity}</Table.Td>
      <Table.Td>{book.shelfLocation || '-'}</Table.Td>
      <Table.Td>
        <Badge color={book.status === 'Available' ? 'green' : 'red'}>
          {book.status === 'Available' ? 'Sẵn sàng' : 'Hết sách'}
        </Badge>
      </Table.Td>
      <Table.Td>
        <Menu shadow="md" width={120}>
          <Menu.Target>
            <ActionIcon variant="subtle" color="gray">
              <IconDotsVertical size="1rem" />
            </ActionIcon>
          </Menu.Target>
          <Menu.Dropdown>
            <Menu.Item leftSection={<IconEdit size="0.8rem" />} onClick={() => handleOpenEdit(book)}>
              Sửa
            </Menu.Item>
            <Menu.Item leftSection={<IconTrash size="0.8rem" />} color="red" onClick={() => handleDelete(book.id)}>
              Xóa
            </Menu.Item>
          </Menu.Dropdown>
        </Menu>
      </Table.Td>
    </Table.Tr>
  ));

  return (
    <Box style={{ position: 'relative', minHeight: '400px' }}>
      <LoadingOverlay visible={loading} overlayProps={{ blur: 1 }} />

      <Group justify="space-between" mb="lg">
        <div>
          <Title order={2} fw={700}>Quản lý đầu sách</Title>
          <Text size="sm" c="dimmed">Xem, cập nhật, xóa và thêm mới các đầu sách của thư viện</Text>
        </div>
        <Button onClick={handleOpenAdd} leftSection={<IconPlus size="1.2rem" />}>
          Thêm sách mới
        </Button>
      </Group>

      <TextInput
        placeholder="Tìm kiếm theo tiêu đề hoặc tác giả..."
        mb="md"
        value={search}
        onChange={(e) => {
          setSearch(e.target.value);
          setPage(1);
        }}
        leftSection={<IconSearch size="1rem" />}
      />

      {error && (
        <Alert icon={<IconAlertCircle size="1rem" />} title="Lỗi thao tác" color="red" mb="md" withCloseButton onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <ScrollArea style={{ minHeight: 300 }}>
        <Table verticalSpacing="sm" highlightOnHover striped>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>Mã sách</Table.Th>
              <Table.Th>Tên sách</Table.Th>
              <Table.Th>Tác giả</Table.Th>
              <Table.Th>Thể loại</Table.Th>
              <Table.Th>NXB (Năm)</Table.Th>
              <Table.Th style={{ textAlign: 'center' }}>Số lượng</Table.Th>
              <Table.Th>Kệ</Table.Th>
              <Table.Th>Trạng thái</Table.Th>
              <Table.Th style={{ width: 50 }}></Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {rows.length > 0 ? rows : (
              <Table.Tr>
                <Table.Td colSpan={9} style={{ textAlign: 'center' }}>
                  <Text c="dimmed" py="xl">Không tìm thấy cuốn sách nào.</Text>
                </Table.Td>
              </Table.Tr>
            )}
          </Table.Tbody>
        </Table>
      </ScrollArea>

      {totalPages > 1 && (
        <Group justify="center" mt="xl">
          <Pagination total={totalPages} value={page} onChange={setPage} />
        </Group>
      )}

      <Modal opened={opened} onClose={close} title={editingBook ? 'Cập nhật thông tin sách' : 'Thêm sách mới'} size="md">
        <form onSubmit={handleSubmit}>
          <Stack gap="md">
            {!editingBook && (
              <TextInput
                label="Mã sách"
                placeholder="Để trống để tự động tạo"
                value={bookCode}
                onChange={(e) => setBookCode(e.target.value)}
              />
            )}
            <TextInput
              label="Tên sách"
              required
              placeholder="Nhập tên sách"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
            <TextInput
              label="Tác giả"
              required
              placeholder="Nhập tên tác giả"
              value={author}
              onChange={(e) => setAuthor(e.target.value)}
            />
            <TextInput
              label="Thể loại"
              required
              placeholder="Ví dụ: Văn học, Khoa học, CNTT"
              value={category}
              onChange={(e) => setCategory(e.target.value)}
            />
            <TextInput
              label="Nhà xuất bản"
              required
              placeholder="Nhập nhà xuất bản"
              value={publisher}
              onChange={(e) => setPublisher(e.target.value)}
            />
            <NumberInput
              label="Năm xuất bản"
              required
              placeholder="Ví dụ: 2026"
              value={publishYear === '' ? undefined : publishYear}
              onChange={(val) => setPublishYear(typeof val === 'number' ? val : '')}
              max={new Date().getFullYear()}
            />
            <NumberInput
              label="Số lượng tồn kho"
              required
              min={0}
              value={quantity}
              onChange={(val) => setQuantity(typeof val === 'number' ? val : 0)}
            />
            <TextInput
              label="Vị trí kệ sách"
              placeholder="Ví dụ: A1-Floor2"
              value={shelfLocation}
              onChange={(e) => setShelfLocation(e.target.value)}
            />
            <Button type="submit" fullWidth mt="md">
              {editingBook ? 'Cập nhật' : 'Lưu lại'}
            </Button>
          </Stack>
        </form>
      </Modal>
    </Box>
  );
}

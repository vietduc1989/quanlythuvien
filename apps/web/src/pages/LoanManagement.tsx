import { useEffect, useState } from 'react';
import {
  Table, ScrollArea, Button, Group, Modal,
  Stack, Pagination, Title, Badge, Box, LoadingOverlay,
  Text, Alert, Select, MultiSelect, Paper, Divider
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconPlus, IconClipboardList, IconAlertCircle, IconArrowBackUp } from '@tabler/icons-react';
import axiosClient from '../api/apiClient';

interface LoanDetail {
  loanDetailId: string;
  bookId: string;
  bookTitle: string;
  actualReturnDate?: string;
  detailStatus: string; // "DangMuon" or "DaTra"
  isOverdue: boolean;
}

interface Loan {
  loanId: string;
  readerId: string;
  readerName: string;
  loanDate: string;
  dueDate: string;
  loanStatus: string;
  details: LoanDetail[];
}

interface ReaderOption {
  readerId: string;
  fullName: string;
  readerCode: string;
}

interface BookOption {
  id: string;
  title: string;
  bookCode: string;
  status: string;
}

export default function LoanManagement() {
  const [loans, setLoans] = useState<Loan[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [isOverdueFilter, setIsOverdueFilter] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form & Dropdown states
  const [opened, { open, close }] = useDisclosure(false);
  const [readers, setReaders] = useState<ReaderOption[]>([]);
  const [books, setBooks] = useState<BookOption[]>([]);
  const [selectedReader, setSelectedReader] = useState<string | null>(null);
  const [selectedBooks, setSelectedBooks] = useState<string[]>([]);
  const [submitLoading, setSubmitLoading] = useState(false);

  const fetchLoans = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await axiosClient.get('/Loans', {
        params: {
          isOverdue: isOverdueFilter === 'true' ? true : isOverdueFilter === 'false' ? false : undefined,
          pageNumber: page,
          pageSize: pageSize,
        },
      });

      if (res.data && res.data.success) {
        setLoans(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      } else {
        setError(res.data.message || 'Không thể tải danh sách phiếu mượn.');
      }
    } catch (err: any) {
      console.error(err);
      setError('Đã xảy ra lỗi khi kết nối với máy chủ.');
    } finally {
      setLoading(false);
    }
  };

  const loadDropdownData = async () => {
    try {
      // Get active readers (status = 0)
      const readersRes = await axiosClient.get('/Readers', { params: { status: 0, pageSize: 100 } });
      if (readersRes.data && readersRes.data.success) {
        setReaders(readersRes.data.data.items || []);
      }

      // Get available books
      const booksRes = await axiosClient.get('/Books', { params: { pageSize: 100 } });
      if (booksRes.data && booksRes.data.success) {
        const allBooks = booksRes.data.data.items || [];
        // Filter out of stock books
        setBooks(allBooks.filter((b: BookOption) => b.status === 'Available'));
      }
    } catch (err) {
      console.error('Failed to load dropdown data:', err);
    }
  };

  useEffect(() => {
    fetchLoans();
  }, [page, isOverdueFilter]);

  const handleOpenAdd = () => {
    setSelectedReader(null);
    setSelectedBooks([]);
    loadDropdownData();
    open();
  };

  const handleCreateLoan = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedReader || selectedBooks.length === 0) {
      setError('Vui lòng chọn độc giả và ít nhất một cuốn sách.');
      return;
    }

    setSubmitLoading(true);
    setError(null);
    try {
      const res = await axiosClient.post('/Loans', {
        readerId: selectedReader,
        bookIds: selectedBooks,
      });

      if (res.data && res.data.success) {
        close();
        setPage(1);
        fetchLoans();
      } else {
        setError(res.data.message || 'Lỗi lập phiếu mượn.');
      }
    } catch (err: any) {
      console.error(err);
      const errMsg = err.response?.data?.message || err.message || 'Không thể lập phiếu mượn.';
      setError(errMsg);
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleReturnBook = async (loanDetailId: string) => {
    if (!confirm('Xác nhận trả cuốn sách này?')) return;
    setLoading(true);
    setError(null);
    try {
      const res = await axiosClient.put(`/Loans/${loanDetailId}/return`);
      if (res.data && res.data.success) {
        fetchLoans();
      } else {
        setError(res.data.message || 'Lỗi khi ghi nhận trả sách.');
      }
    } catch (err: any) {
      console.error(err);
      setError(err.response?.data?.message || 'Không thể thực hiện trả sách.');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateStr: string) => {
    if (!dateStr) return '-';
    return new Date(dateStr).toLocaleDateString('vi-VN');
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <Box style={{ position: 'relative', minHeight: '400px' }}>
      <LoadingOverlay visible={loading} overlayProps={{ blur: 1 }} />

      <Group justify="space-between" mb="lg">
        <div>
          <Title order={2} fw={700}>Quản lý mượn trả sách</Title>
          <Text size="sm" c="dimmed">Lập phiếu mượn sách mới và theo dõi, ghi nhận trả sách quá hạn</Text>
        </div>
        <Button onClick={handleOpenAdd} color="indigo" leftSection={<IconPlus size="1.2rem" />}>
          Lập phiếu mượn mới
        </Button>
      </Group>

      <Select
        placeholder="Lọc theo tình trạng"
        mb="md"
        value={isOverdueFilter}
        onChange={(val) => {
          setIsOverdueFilter(val);
          setPage(1);
        }}
        data={[
          { value: 'true', label: 'Quá hạn chưa trả' },
          { value: 'false', label: 'Trong hạn / Đã trả đủ' },
        ]}
        clearable
        style={{ maxWidth: 300 }}
      />

      {error && (
        <Alert icon={<IconAlertCircle size="1rem" />} title="Thông báo" color="red" mb="md" withCloseButton onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <ScrollArea style={{ minHeight: 300 }}>
        {loans.length > 0 ? (
          <Stack gap="md">
            {loans.map((loan) => (
              <Paper key={loan.loanId} p="md" withBorder shadow="sm">
                <Group justify="space-between" mb="xs">
                  <div>
                    <Group gap="xs">
                      <IconClipboardList size="1.1rem" style={{ color: '#4c6ef5' }} />
                      <Text fw={600} size="sm">{loan.readerName}</Text>
                      <Text size="xs" c="dimmed">({loan.loanId.substring(0, 8).toUpperCase()})</Text>
                    </Group>
                    <Text size="xs" c="dimmed" mt={2}>
                      Ngày mượn: {formatDate(loan.loanDate)} | Hạn trả: <span style={{ color: new Date(loan.dueDate) < new Date() && loan.details.some(d => d.detailStatus === 'DangMuon') ? 'red' : 'inherit', fontWeight: 600 }}>{formatDate(loan.dueDate)}</span>
                    </Text>
                  </div>
                  <Badge color={loan.details.every(d => d.detailStatus === 'DaTra') ? 'green' : 'blue'}>
                    {loan.details.every(d => d.detailStatus === 'DaTra') ? 'Đã trả hết' : 'Đang mượn'}
                  </Badge>
                </Group>
                <Divider my="sm" />
                <Table verticalSpacing="xs">
                  <Table.Thead>
                    <Table.Tr>
                      <Table.Th style={{ fontSize: '12px' }}>Tên cuốn sách</Table.Th>
                      <Table.Th style={{ fontSize: '12px', width: '150px' }}>Trạng thái sách</Table.Th>
                      <Table.Th style={{ fontSize: '12px', width: '150px' }}>Ngày trả thực tế</Table.Th>
                      <Table.Th style={{ fontSize: '12px', width: '100px', textAlign: 'right' }}></Table.Th>
                    </Table.Tr>
                  </Table.Thead>
                  <Table.Tbody>
                    {loan.details.map((detail) => (
                      <Table.Tr key={detail.loanDetailId}>
                        <Table.Td style={{ fontSize: '13px' }}>{detail.bookTitle}</Table.Td>
                        <Table.Td style={{ fontSize: '13px' }}>
                          {detail.detailStatus === 'DangMuon' ? (
                            <Badge color={detail.isOverdue ? 'red' : 'yellow'} variant="light" size="xs">
                              {detail.isOverdue ? 'Quá hạn' : 'Đang mượn'}
                            </Badge>
                          ) : (
                            <Badge color="green" variant="light" size="xs">Đã trả</Badge>
                          )}
                        </Table.Td>
                        <Table.Td style={{ fontSize: '13px' }}>{formatDate(detail.actualReturnDate || '')}</Table.Td>
                        <Table.Td style={{ textAlign: 'right' }}>
                          {detail.detailStatus === 'DangMuon' && (
                            <Button
                              size="xs"
                              variant="light"
                              color="green"
                              onClick={() => handleReturnBook(detail.loanDetailId)}
                              leftSection={<IconArrowBackUp size="0.9rem" />}
                            >
                              Trả sách
                            </Button>
                          )}
                        </Table.Td>
                      </Table.Tr>
                    ))}
                  </Table.Tbody>
                </Table>
              </Paper>
            ))}
          </Stack>
        ) : (
          <Paper p="xl" withBorder style={{ textAlign: 'center' }}>
            <Text c="dimmed">Không tìm thấy phiếu mượn nào.</Text>
          </Paper>
        )}
      </ScrollArea>

      {totalPages > 1 && (
        <Group justify="center" mt="xl">
          <Pagination total={totalPages} value={page} onChange={setPage} />
        </Group>
      )}

      <Modal opened={opened} onClose={close} title="Lập phiếu mượn sách mới" size="md">
        <form onSubmit={handleCreateLoan}>
          <Stack gap="md">
            <Select
              label="Chọn độc giả"
              placeholder="Gõ để tìm kiếm..."
              required
              searchable
              value={selectedReader}
              onChange={setSelectedReader}
              data={readers.map(r => ({ value: r.readerId, label: `${r.fullName} (${r.readerCode})` }))}
            />

            <MultiSelect
              label="Chọn sách mượn"
              placeholder="Chọn một hoặc nhiều cuốn sách..."
              required
              searchable
              value={selectedBooks}
              onChange={setSelectedBooks}
              data={books.map(b => ({ value: b.id, label: `${b.title} (${b.bookCode})` }))}
              maxValues={5} // Giới hạn mượn tối đa 5 cuốn
            />

            <Button type="submit" color="indigo" fullWidth mt="md" loading={submitLoading}>
              Lập phiếu mượn
            </Button>
          </Stack>
        </form>
      </Modal>
    </Box>
  );
}
